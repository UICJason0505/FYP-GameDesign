using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static HexMath;

public class Knight : Chess
{
    [Header("Push Skill")]
    public KeyCode pushKey = KeyCode.C;
    public int pushCooldownTurns = 2;

    private bool isPushMode = false;
    private int pushCooldownRemain = 0;
    private bool lastCanOperate = false;

    // 保存进入推击模式前的状态
    private int savedAttackArea;
    private bool savedAttackModeValid = false;
    private bool savedSelectionAttackMode;
    private bool savedSelectionAttackModeValid = false;

    // 备份推击范围内 tile 的原始 attackable，用于退出时复原
    private readonly Dictionary<HexTile, bool> pushAttackableBackup = new();
    private bool pushBackupSaved = false;

    // GM tiles 索引（用于找 C / D 是否存在）
    private readonly Dictionary<(int x, int z), HexTile> tileMap = new();

    // 推击模式时临时把所有棋子 collider 设为 IgnoreRaycast（2），这里存原 layer
    private readonly Dictionary<GameObject, int> savedLayers = new();

    // 反射：GridBuildingSystem.private TryGetHoveredTile(out HexTile)
    private MethodInfo gbsTryGetHoveredTileMI;

    private static readonly Coordinates[] dirs = new Coordinates[]
    {
        new Coordinates(1, 0),
        new Coordinates(1, -1),
        new Coordinates(0, -1),
        new Coordinates(-1, 0),
        new Coordinates(-1, 1),
        new Coordinates(0, 1),
    };

    protected override void Update()
    {
        // ===== 1) 推击模式逻辑放在 base.Update() 前（你要求的）=====

        bool myTurnNow = (player != null && player.canOperate);

        // 自己回合刚开始：冷却 -1
        if (myTurnNow && !lastCanOperate)
        {
            if (pushCooldownRemain > 0) pushCooldownRemain--;
        }
        lastCanOperate = myTurnNow;

        bool selected = (SelectionManager.selectedObj == gameObject || MovingObject.selectedObj == gameObject);

        // 不满足条件就退出
        if (!myTurnNow || !selected)
        {
            if (isPushMode) ExitPushMode();
        }
        else
        {
            // 切换推击模式
            if (Input.GetKeyDown(pushKey))
            {
                if (isPushMode) ExitPushMode();
                else EnterPushMode();
            }

            if (isPushMode)
            {
                // 让 baseUpdate 的攻击范围系统能画：把关键字段先设置好
                if (!savedAttackModeValid)
                {
                    savedAttackArea = attackArea;
                    savedAttackModeValid = true;
                }

                // 推击范围固定 1 格
                attackArea = 1;

                // 如果 Chess.showAttackableTiles 依赖 position / tiles，这里同步一下
                position = WorldToCoordinates(transform.position, HexTile.radius);
                if (GameManager.Instance != null && GameManager.Instance.tiles != null)
                    tiles = GameManager.Instance.tiles;

                // 推击用攻击范围展示（你要求用 showAttackableTiles）
                // 注意：这个函数来自 Chess，不要在这里重写同名函数
                showAttackableTiles();
                DisableAttackableIfOccupiedInPushRange();

                // 防止 SelectionManager 左键选择逻辑影响（可选，但更稳）
                if (!savedSelectionAttackModeValid)
                {
                    savedSelectionAttackMode = SelectionManager.isAttackMode;
                    savedSelectionAttackModeValid = true;
                }
                SelectionManager.isAttackMode = true;

                // 取消
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    ExitPushMode();
                }
                // 左键：选相邻格 B 定方向（鼠标只能调用 TryGetHoveredTile）
                else if (Input.GetMouseButtonDown(0))
                {
                    if (TryGetHoveredByGBS(out HexTile tileB) && tileB != null)
                    {
                        if (tileB.isOccupied)
                        {
                            UnitInfoPanelController.Instance?.ShowInvalidAction("Blocked: clicked tile occupied");
                            return;
                        }
                        // 必须点在范围内：showAttackableTiles 会把 attackable=true 的格子染色
                        if (tileB.attackable)
                        {
                            bool success = TryExecutePush(tileB.coordinates);
                            if (success) pushCooldownRemain = pushCooldownTurns;

                            // 你要求：成功/失败（尝试结束）都会 ResetTiles 并结束
                            ExitPushMode();
                        }
                    }
                }
            }
        }

        // ===== 2) 必须调用 base.Update()（不动你的攻击/防御体系）=====
        base.Update();
    }

    private void EnterPushMode()
    {
        if (pushCooldownRemain > 0)
        {
            UnitInfoPanelController.Instance?.ShowInvalidAction($"Cooldown: {pushCooldownRemain}");
            return;
        }

        CacheTilesFromGameManager();
        BackupPushRangeAttackable();

        // 进入推击模式：临时让棋子 collider 不挡住 TryGetHoveredTile 的 Raycast
        BeginIgnoreChessRaycast();

        isPushMode = true;
    }

    private void ExitPushMode()
    {
        isPushMode = false;

        // 结束：恢复棋子 layer
        EndIgnoreChessRaycast();
        RestorePushRangeAttackable();
        // 结束：调用 Chess 的 ResetTiles（你说 ResetTiles 是 Chess 的）
        ResetTiles();

        // 恢复 attackArea
        if (savedAttackModeValid)
        {
            attackArea = savedAttackArea;
            savedAttackModeValid = false;
        }

        // 恢复 SelectionManager.isAttackMode
        if (savedSelectionAttackModeValid)
        {
            SelectionManager.isAttackMode = savedSelectionAttackMode;
            savedSelectionAttackModeValid = false;
        }
    }

    // 选 B -> 推 C -> 落到 D
    private bool TryExecutePush(Coordinates b)
    {
        CacheTilesFromGameManager();
        HexTile.RefreshAllChess();

        Coordinates a = WorldToCoordinates(transform.position, HexTile.radius);
        Coordinates dir = new Coordinates(b.x - a.x, b.z - a.z);

        if (!IsValidDir(dir) || HexDistance(a, b) != 1)
        {
            UnitInfoPanelController.Instance?.ShowInvalidAction("Invalid direction");
            return false;
        }

        // C
        Coordinates c = new Coordinates(b.x + dir.x, b.z + dir.z);
        if (!tileMap.TryGetValue((c.x, c.z), out HexTile tileC) || tileC == null)
        {
            UnitInfoPanelController.Instance?.ShowInvalidAction("No HexTile ahead");
            return false;
        }

        // 必须用 isOccupied
        if (!tileC.isOccupied)
        {
            UnitInfoPanelController.Instance?.ShowInvalidAction("No unit to push");
            return false;
        }

        Chess pushed = tileC.occupyingChess;
        if (pushed == null) pushed = HexTile.GetChessAt(c);
        if (pushed == null)
        {
            UnitInfoPanelController.Instance?.ShowInvalidAction("No unit to push");
            return false;
        }

        if (pushed.player == this.player)
        {
            UnitInfoPanelController.Instance?.ShowInvalidAction("Can't push ally");
            return false;
        }

        // D
        Coordinates d = new Coordinates(c.x + dir.x, c.z + dir.z);

        // 先拿 B（用来算“落点 worldPos”，因为 D 可能没 HexTile）
        tileMap.TryGetValue((b.x, b.z), out HexTile tileB);
        Vector3 step = (tileB != null) ? (tileC.centerWorld - tileB.centerWorld) : Vector3.zero;
        Vector3 destWorld = tileC.centerWorld + step; // 这是“理论上的 D 位置”（即使没 tile）

        bool hasTileD = tileMap.TryGetValue((d.x, d.z), out HexTile tileD) && tileD != null;
        bool destIsWater = false;

        // 情况1：D 有 tile，直接看 tag
        if (hasTileD)
        {
            destIsWater = tileD.CompareTag("Water");
            destWorld = tileD.centerWorld; // 有 tile 就用它的中心
        }
        else
        {
            // 情况2：D 没 tile，用 Raycast 看落点下方是否是 Water
            // 注意：你在推击模式里已经把所有 Chess collider 设成 IgnoreRaycast 了，所以这里不会被棋子挡住
            Ray ray = new Ray(destWorld + Vector3.up * 5f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f))
            {
                if (hit.collider != null && hit.collider.CompareTag("Water"))
                    destIsWater = true;
            }
        }

        // 不是水：必须有 tileD 且 tileD 空
        if (!destIsWater)
        {
            if (!hasTileD)
            {
                UnitInfoPanelController.Instance?.ShowInvalidAction("Blocked: no HexTile behind");
                return false;
            }

            if (tileD.isOccupied)
            {
                UnitInfoPanelController.Instance?.ShowInvalidAction("Blocked: destination occupied");
                return false;
            }
        }

        // ===== 执行推击：不加不扣 AP =====
        tileC.isOccupied = false;
        tileC.occupyingChess = null;

        if (destIsWater)
        {
            // 推下水：允许推，但不占用 D
            pushed.position = d;
            pushed.transform.position = destWorld + Vector3.up * 0.25f;

            StartCoroutine(DieRoutine(pushed)); // 推下水死亡（如果你不想死，把这行删掉）
            HexTile.RefreshAllChess();
            // ===== 推完后自己走到点击的 B（不加数字）=====
            if (tileMap.TryGetValue((a.x, a.z), out HexTile tileE) && tileE != null && tileE.occupyingChess == this)
            {
                tileE.isOccupied = false;
                tileE.occupyingChess = null;
            }

            tileB.isOccupied = true;
            tileB.occupyingChess = this;

            position = b;
            transform.position = tileB.centerWorld + Vector3.up * 0.25f;
            return true;
        }

        // 普通落地：D 必须存在且空
        tileD.isOccupied = true;
        tileD.occupyingChess = pushed;

        pushed.position = d;
        pushed.transform.position = tileD.centerWorld + Vector3.up * 0.25f;
        HexTile.RefreshAllChess();
        // ===== 推完后骑士走到点击的 B（不加数字）=====
        if (tileMap.TryGetValue((a.x, a.z), out HexTile tileF) && tileF!= null && tileF.occupyingChess == this)
        {
            tileF.isOccupied = false;
            tileF.occupyingChess = null;
        }

        tileB.isOccupied = true;
        tileB.occupyingChess = this;

        position = b;
        transform.position = tileB.centerWorld + Vector3.up * 0.25f;

        return true;
    }

    private bool IsValidDir(Coordinates dir)
    {
        for (int i = 0; i < dirs.Length; i++)
            if (dirs[i].x == dir.x && dirs[i].z == dir.z) return true;
        return false;
    }

    private void CacheTilesFromGameManager()
    {
        tileMap.Clear();
        if (GameManager.Instance == null || GameManager.Instance.tiles == null) return;

        foreach (var t in GameManager.Instance.tiles)
        {
            if (t == null) continue;
            tileMap[(t.coordinates.x, t.coordinates.z)] = t;
        }
    }

    // 鼠标相关：只能调用 GridBuildingSystem.private TryGetHoveredTile(out HexTile)
    private bool TryGetHoveredByGBS(out HexTile tile)
    {
        tile = null;
        if (GridBuildingSystem.Instance == null) return false;

        if (gbsTryGetHoveredTileMI == null)
        {
            gbsTryGetHoveredTileMI = typeof(GridBuildingSystem).GetMethod(
                "TryGetHoveredTile",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            if (gbsTryGetHoveredTileMI == null) return false;
        }

        object[] args = new object[] { null };
        bool ok = (bool)gbsTryGetHoveredTileMI.Invoke(GridBuildingSystem.Instance, args);
        tile = args[0] as HexTile;
        return ok && tile != null;
    }

    // 进入推击模式：让 Raycast 更容易命中 HexTile（不改 GridBuildingSystem）
    private void BeginIgnoreChessRaycast()
    {
        savedLayers.Clear();

        Chess[] all = FindObjectsOfType<Chess>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] == null) continue;

            Collider[] cols = all[i].GetComponentsInChildren<Collider>(true);
            for (int k = 0; k < cols.Length; k++)
            {
                if (cols[k] == null) continue;
                GameObject go = cols[k].gameObject;

                if (!savedLayers.ContainsKey(go))
                    savedLayers.Add(go, go.layer);

                go.layer = 2; // Ignore Raycast
            }
        }
    }

    private void EndIgnoreChessRaycast()
    {
        foreach (var kv in savedLayers)
        {
            if (kv.Key != null) kv.Key.layer = kv.Value;
        }
        savedLayers.Clear();
    }

    // ==========================
    // 你说“攻击和防御函数不要动”
    // 下面两段完全按你给的原样保留
    // ==========================
    public override int attack()
    {
        StartCoroutine(AttackRoutine(this));
        return number;
    }

    public override void defend(int damage, Chess attacker, Chess target)
    {
        number -= damage;
        Debug.Log($"{name} 受到伤害：{damage} 剩余 {number}");
        if (HexMath.HexDistance(attacker.position, this.position) == 1)
        {
            attacker.number -= number;
            Debug.Log($"{name} 反击 {attacker.name}，造成 {number} 伤害");
            StartCoroutine(AttackRoutine(target));
        }
        if (number <= 0) StartCoroutine(DieRoutine(target));
        if (attacker.number <= 0) StartCoroutine(DieRoutine(attacker));
    }
    // 备份：按C那一刻，把“相邻一格(推击范围)”的 attackable 原值存起来
    private void BackupPushRangeAttackable()
    {
        pushAttackableBackup.Clear();

        Coordinates a = WorldToCoordinates(transform.position, HexTile.radius);

        for (int i = 0; i < dirs.Length; i++)
        {
            Coordinates b = new Coordinates(a.x + dirs[i].x, a.z + dirs[i].z);
            if (tileMap.TryGetValue((b.x, b.z), out HexTile tile) && tile != null)
            {
                if (!pushAttackableBackup.ContainsKey(tile))
                    pushAttackableBackup.Add(tile, tile.attackable);
            }
        }

        pushBackupSaved = true;
    }

    // 过滤：把范围内“已占用”的格子设为不可点
    private void DisableAttackableIfOccupiedInPushRange()
    {
        if (!pushBackupSaved) return;

        foreach (var kv in pushAttackableBackup)
        {
            HexTile t = kv.Key;
            if (t == null) continue;

            if (t.isOccupied)
            {
                t.attackable = false;

                // 可选：如果你不想占用格子还高亮，就取消高亮
                t.ResetTile();
            }
        }
    }

    // 复原：退出推击模式时把 attackable 恢复
    private void RestorePushRangeAttackable()
    {
        if (!pushBackupSaved) return;

        foreach (var kv in pushAttackableBackup)
        {
            if (kv.Key != null) kv.Key.attackable = kv.Value;
        }

        pushAttackableBackup.Clear();
        pushBackupSaved = false;
    }
}

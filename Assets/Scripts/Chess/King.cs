using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
//private bool isSelected = false;
public class King : Chess
{
    [Header("基础属性")]
    public TurnManager turnManager;
    private bool isInSummonMode = false;
    int attackPower;
    int choose = -1;
    public void Awake()
    {
        var go = GameObject.Find("GameManager");           
        if (go != null) move = go.GetComponent<MovingObject>();
        if (panel == null)
        {
            panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault(); ;
        }

        // 尝试稳健地获取 MovingObject，避免 move 为 null 导致按键无效或 NRE
        if (move == null)
        {
            move = FindObjectOfType<MovingObject>();
        }
        panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault();
        turnManager = FindObjectOfType<TurnManager>();
        attackPower = int.MaxValue;
    }
    public void Start()
    {
        blood = 5;
    }
    void OnMouseDown()
    {
        if (panel != null)
        {
            panel.ShowUnit(player.playerName, blood);
        }
    }

    public override void KingAttack(Chess attacker, Chess target)
    {
        int layer = target.gameObject.layer;
        int saberLayer = LayerMask.NameToLayer("Saber");
        int knightLayer = LayerMask.NameToLayer("Knight");
        int peasantLayer = LayerMask.NameToLayer("Peasant");
        int archerLayer = LayerMask.NameToLayer("Archer");
        int shielderLayer = LayerMask.NameToLayer("ShieldGuard");
        int cannoneerLayer = LayerMask.NameToLayer("Cannoneer");
        int kingLayer = LayerMask.NameToLayer("King");
        switch (layer)
        {
            // 近战普攻：Saber / Knight / Peasant / Shielder—— 完全相同逻辑，合并
            case int _ when layer == saberLayer || layer == knightLayer || layer == peasantLayer|| layer == shielderLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    attacker.blood --;
                    target.number -= attackPower;
                    StartCoroutine(AttackRoutine(attacker));
                    StartCoroutine(AttackRoutine(target));
                    // 更新面板
                    if (panel != null)
                    {
                        panel.ShowUnit(attacker.gameObject.name, attacker.number);
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    // 判定死亡
                    KingDeathCheck();
                    CheckDeath(target);
                    break;
                }

            case int _ when layer == cannoneerLayer || layer == archerLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;

                    target.number -= attackPower;
                    StartCoroutine(AttackRoutine(attacker));
                    if (panel != null)
                    {
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    CheckDeath(target);
                    break;
                }
            case int _ when layer == kingLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    attacker.blood--;
                    target.blood --;
                    StartCoroutine(AttackRoutine(attacker));
                    StartCoroutine(AttackRoutine(target));
                    // 更新面板
                    if (panel != null)
                    {
                        panel.ShowUnit(attacker.gameObject.name, attacker.number);
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    // 判定死亡
                    KingDeathCheck();
                    target.KingDeathCheck();
                    break;
                }
        }
    }

    // 统一处理死亡
    public override void  CheckDeath(Chess unit)
    {
        if (unit.number <= 0)
        {
            StartCoroutine(DieRoutine(unit));
        }
    }
    public override void KingDeathCheck()
    {
        if (this.blood <= 0)
        {
            for(int i = 0; i < turnManager.players.Count; i++)
            {
                if(turnManager.players[i] == this.player)
                {
                    turnManager.players.RemoveAt(i);
                    break;
                }
            }
            Chess[] chess = FindObjectsOfType<Chess>();
            for (int i = 0; i < chess.Length; i++)
            {
                if(chess[i] != this && chess[i].player == this.player)
                {
                    StartCoroutine(DieRoutine(chess[i]));
                }
            }
            StartCoroutine(DieRoutine(this));
        }
    }
    public override void KingDefend(Chess attacker, Chess target)
    {
        print("King Defend called");
        int aBefore = attacker.number;
        int bBefore = target.number;

        attacker.number -= attackPower;
        target.blood --;
        StartCoroutine(AttackRoutine(attacker));
        StartCoroutine(AttackRoutine(target));
        Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {1} 敌方剩余血量{target.number}");
        // 更新面板
        if (panel != null)
        {
            panel.ShowUnit(attacker.gameObject.name, attacker.number);
            panel.ShowUnit(target.gameObject.name, target.number);
        }

        // 判定死亡
        target.KingDeathCheck();
        CheckDeath(attacker);
    }
    protected override void Update()
    {
        if (currentTile.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            this.blood -= 5;
            KingDeathCheck();
        }
        // 只有当前选中的 King 才能响应 C 键和召唤
        if (SelectionManager.selectedObj != gameObject)
        {
            base.Update();
            return;
        }

        // 已经在召唤模式中：每帧处理召唤输入
        if (isInSummonMode)
        {
            UpdateSummonMode();
            return;
        }

        // 不在召唤模式：正常逻辑 + 监听进入键
        if (Input.GetKeyDown(KeyCode.C) && !isInAttackMode)
        {
            EnterSummonMode();
            //FYJ召唤UI
            // panel.subPanelUI.TogglePanel();       ====TogglePanel() 不存在，暂时注释====
            return;
        }
        base.Update();
    }
    void EnterSummonMode()
    {
        isInSummonMode = true;
        choose = -1;
        ShowSummonArea();   // 高亮可召唤区域

        panel.subPanelUI.ShowPanel();
    }

    void ExitSummonMode()
    {
        isInSummonMode = false;
        ResetTiles();       // 清除高亮
        panel.subPanelUI.HidePanel();
    }
    void UpdateSummonMode()
    {
        // 再按一次 C：取消召唤
        if (Input.GetKeyDown(KeyCode.C))
        {
            ExitSummonMode();
            return;
        }

        // 选择兵种（这里示例 1~6）
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) choose = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) choose = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) choose = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) choose = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) choose = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) choose = 6;

        // 鼠标左键：尝试在点击的格子上召唤
        if (Input.GetMouseButtonDown(0))
        {
            // 没有选择兵种，直接退出
            if (choose == -1)
            {
                ExitSummonMode();
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int tileMask = 1 << LayerMask.NameToLayer("Tile");

            if (Physics.Raycast(ray, out hit, 1000f, tileMask))
            {
                HexTile tile = hit.collider.GetComponent<HexTile>();

                if (tile != null && tile.attackable)   // 或者你自己的条件，比如 tile.canSummon
                {
                    // 注意 prefab 下标是 0 开始，所以要 -1
                    if(choose == 1 ||choose == 2)
                    {
                        if (player.HasEnoughActionPoints(1))
                        {
                            GameObject prefab = gameManager.prefabs[choose - 1];
                            var chess = Instantiate(prefab, tile.centerWorld + Vector3.up * 0.25f, Quaternion.identity);
                            chess.GetComponent<Chess>().player = this.player;
                            chess.GetComponent<Renderer>().material.color = ColorFromName(this.player.playerName);
                            player.actionPoints -= 1;
                        }
                    }
                    if (choose == 3 || choose == 4 || choose == 5 || choose == 6)
                    {
                        if (player.HasEnoughActionPoints(2))
                        {
                            GameObject prefab = gameManager.prefabs[choose - 1];
                            var chess = Instantiate(prefab, tile.centerWorld + Vector3.up * 0.6f, Quaternion.identity);
                            chess.GetComponent<Chess>().player = this.player;
                            chess.GetComponent<Renderer>().material.color = ColorFromName(this.player.playerName);
                            player.actionPoints -= 2;
                        }
                    }
                }
            }

            HexTile.RefreshAllChess(); // 刷新场上棋子信息
            ExitSummonMode();
        }
    }
    public void ShowSummonArea()
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            int distX = tile.coordinates.x - position.x;
            int distZ = tile.coordinates.z - position.z;
            int dist = distX + distZ;
            if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 <= attackArea)
            {
                tile.HighlightTile();
            }
        }
    }
    private void Die()
    {
    }
    Color ColorFromName(string name)
    {
        switch (name)
        {
            case "Red": return Color.red;
            case "Green": return Color.green;
            case "Blue": return Color.blue;
            case "Yellow": return Color.yellow;
            default:
                Debug.LogWarning("未知颜色名: " + name + "，默认用白色");
                return Color.white;
        }
    }
}

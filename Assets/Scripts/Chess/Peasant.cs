using System.Linq;
using UnityEngine;
using static MovingObject;

public class Peansant : Chess
{
    [Header("Peansant属性")]
    private int initialValue = 3; // 初始数值
    private int attackDistance = 1;
    TurnManager turnManager; // Awake 用于获取全局引用，避免 move 为 null
    private bool skillSelecting = false;

    private void Awake()
    {
        // 初始化游戏管理器引用
        var go = GameObject.Find("GameManager");
        if (go != null) move = go.GetComponent<MovingObject>();
        if (panel == null)
        {
            panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault();
        }
        var temp = FindObjectOfType<TurnManager>();
        turnManager = temp.GetComponent<TurnManager>(); //添加 TurnManager 初始化
    }

    protected override void Start()
    {
        // 设置单位基础属性
        number = initialValue;
        attackArea = attackDistance;

        // 根据游戏对象名称分配玩家阵营
        if (this.gameObject.name == "Peansant")
        {
            player = turnManager.players[2]; // Red, Blue, |Green|, Yellow 
        }
        else
        {
            player = turnManager.players[3]; // Red, Blue, Green, |Yellow| 
        }
    }

    protected override void Update()
    {
        base.Update();

        // 按下 C 进入技能选择状态
        if (Input.GetKeyDown(KeyCode.C))
        {
            skillSelecting = true;
            showAttackableTiles();
            Debug.Log("Peansant 准备使用技能：选择格子");
        }

        // 鼠标点击格子时
        if (skillSelecting && Input.GetMouseButtonDown(0))
        {
            TrySelectTile();
        }
    }

    private void TrySelectTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        HexTile tile = hit.collider.GetComponent<HexTile>();
        if (tile == null) return;

        // ★ 技能效果：tileValue + 1
        tile.tileValue += 1;
        Debug.Log($"Peansant 技能：格子({tile.coordinates.x},{tile.coordinates.z}) tileValue 现在 = {tile.tileValue}");
        StartCoroutine(SkillRoutine(this));
        // 技能使用完毕
        ResetTiles();
        skillSelecting = false;
    }

    public override int attack()
    {
        StartCoroutine(AttackRoutine(this));
        return this.number;
    }

    public override void defend(int damage, Chess attacker, Chess target)
    {
        int layer = attacker.gameObject.layer;
        int saberLayer = LayerMask.NameToLayer("Saber");
        int knightLayer = LayerMask.NameToLayer("Knight");
        int peasantLayer = LayerMask.NameToLayer("Peasant");
        int archerLayer = LayerMask.NameToLayer("Archer");
        int shielderLayer = LayerMask.NameToLayer("ShieldGuard");
        int cannoneerLayer = LayerMask.NameToLayer("Cannoneer");
        switch (layer)
        {
            // 近战普攻：Saber / Knight / Peasant / Shielder—— 完全相同逻辑，合并
            case int _ when layer == saberLayer || layer == knightLayer || layer == peasantLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    attacker.number -= number;
                    target.number -= damage;
                    StartCoroutine(AttackRoutine(target));
                    // 更新面板
                    if (panel != null)
                    {
                        panel.ShowUnit(attacker.gameObject.name, attacker.number);
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    // 判定死亡
                    if (attacker.number <= 0)
                    {
                        StartCoroutine(DieRoutine(attacker));
                        Debug.Log($"{attacker.name} 被击败！");
                    }
                    if (target.number <= 0)
                    {
                        StartCoroutine(DieRoutine(target));
                        Debug.Log($"{target.name} 被击败！");
                    }
                    break;
                }

            case int _ when layer == cannoneerLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    target.number -= damage;

                    if (panel != null)
                    {
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    if (target.number <= 0)
                    {
                        StartCoroutine(DieRoutine(target));
                        Debug.Log($"{target.name} 被击败！");
                    }
                    break;
                }
            case int _ when layer == archerLayer:
                {
                    int distX = attacker.position.x - target.position.x;
                    int distZ = attacker.position.z - target.position.z;
                    int dist = distX + distZ;
                    if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 <= 1)
                    {
                        int aBefore = attacker.number;
                        int bBefore = target.number;
                        StartCoroutine(AttackRoutine(target));
                        attacker.number -= bBefore;
                        target.number -= damage;

                        Debug.Log($"{attacker.name} 攻击 {target.name}：我方减 {bBefore}，敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

                        if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number); // 更新自己面板
                        if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);
                        if (attacker.number <= 0)
                        {
                            StartCoroutine(DieRoutine(attacker));
                            Debug.Log($"{attacker.name} 被击败！");
                        }
                        if (target.number <= 0)
                        {
                            StartCoroutine(DieRoutine(target));
                            Debug.Log($"{target.name} 被击败！");
                        }
                    }
                    else if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 <= 2)
                    {
                        int aBefore = attacker.number;
                        int bBefore = target.number;
                        target.number -= damage;

                        Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

                        if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);

                        if (target.number <= 0)
                        {
                            StartCoroutine(DieRoutine(target));
                            Debug.Log($"{target.name} 被击败！");
                        }
                    }
                    break;
                }
            case int _ when layer == shielderLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    attacker.number -= (int)(number * 0.5);
                    target.number -= damage;
                    StartCoroutine(AttackRoutine(target));
                    // 更新面板
                    if (panel != null)
                    {
                        panel.ShowUnit(attacker.gameObject.name, attacker.number);
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    // 判定死亡
                    if (attacker.number <= 0)
                    {
                        StartCoroutine(DieRoutine(attacker));
                        Debug.Log($"{attacker.name} 被击败！");
                    }
                    if (target.number <= 0)
                    {
                        StartCoroutine(DieRoutine(target));
                        Debug.Log($"{target.name} 被击败！");
                    }
                    break;
                }
        }
    }
}
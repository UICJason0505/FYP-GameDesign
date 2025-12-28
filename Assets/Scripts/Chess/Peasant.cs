using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Peansant : Chess
{
    [Header("Peansant State")]
    private int initialValue = 3; // 初始数值
    private int attackDistance = 1;
    private int skillDistance = 1;
    private bool skillSelecting = false;

    // 添加数字预制体引用
    [Header("Number Prefab")]
    public GameObject number2Prefab; // 数字2
    public GameObject number3Prefab; // 数字3

    // 记录每个地块上的数字对象
    private Dictionary<HexTile, GameObject> tileNumberObjects = new Dictionary<HexTile, GameObject>();

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
    }

    protected override void Update()
    {
        base.Update();

            // 按下 C 进入技能选择状态
        if (Input.GetKeyDown(KeyCode.C) && SelectionManager.selectedObj == gameObject)
        {
            if (skillSelecting == false)
            {
                skillSelecting = true;
                showAttackableTiles();
                Debug.Log("Peansant 准备使用技能：选择格子");
            }
            else if (skillSelecting == true)
            {
                skillSelecting = false;
                ResetTiles();
                return; 
            }    
        }
        // 鼠标点击格子时
        if (skillSelecting && Input.GetMouseButtonDown(0))
        {
            TrySelectTile();
        }

        if (skillSelecting && Input.GetMouseButtonDown(1))
        {
            skillSelecting = false;
            ResetTiles();
        }
    }

    private void TrySelectTile()
    {
        // 射线检测获取鼠标点击的格子
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        HexTile tile = hit.collider.GetComponent<HexTile>();
        if (tile == null) return;

        // 检查地块是否在技能范围内
        int distX = tile.coordinates.x - position.x;
        int distZ = tile.coordinates.z - position.z;
        int dist = distX + distZ;
        if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 > skillDistance)
        {
            Debug.Log("地块不在技能范围内！");
            return;
        }

        // 使用技能消耗两点AP
        if (player.HasEnoughActionPoints(2))
        {
            if (tile.tileValue >= 3)
            {
                Debug.Log("该地块已达到最大数值3，无法再次使用技能！");
                skillSelecting = false;
                ResetTiles();
                return;
            }
            // 技能效果：tileValue + 1
            tile.tileValue += 1;
            Debug.Log($"Peansant使用了技能！使格子({tile.coordinates.x},{tile.coordinates.z}) 的tileValue变为： {tile.tileValue}！");
            player.actionPoints -= 2;
            StartCoroutine(SkillRoutine(this)); // 播放动画

            // 更新地块上的数字显示
            UpdateNumberOnTile(tile);

            // 技能使用完毕
            ResetTiles();
            skillSelecting = false;
        }
    }

    // 更新地块上的数字y预制体显示
    private void UpdateNumberOnTile(HexTile tile)
    {
        // 移除旧的数字对象
        if (tileNumberObjects.ContainsKey(tile) && tileNumberObjects[tile] != null)
        {
            Destroy(tileNumberObjects[tile]);
        }

        // 根据tileValue选择要显示的数字预制体
        GameObject numberPrefab = null;
        if (tile.tileValue == 2)
        {
            numberPrefab = number2Prefab;
        }
        else if (tile.tileValue == 3)
        {
            numberPrefab = number3Prefab;
        }
        else
        {
            // 如果tileValue不是2或3，就不显示数字
            return;
        }

        if (numberPrefab != null)
        {
            // 设置位置
            Vector3 spawnPosition = new Vector3(
                tile.centerWorld.x - 0.24f, // 左右 加往右，减往左
                0.28f,                      // 高低
                tile.centerWorld.z - 0.43f  // 上下 加往上，减往下
            );

            // 设置旋转角度：x旋转270度，y旋转180度
            Quaternion spawnRotation = Quaternion.Euler(270, 180, 0);

            GameObject numberObj = Instantiate(numberPrefab, spawnPosition, spawnRotation);

            // 缩小一点，使棋子能盖住这个数字
            numberObj.transform.localScale = Vector3.one * 0.85f;

            // 记录这个数字对象
            tileNumberObjects[tile] = numberObj;
        }
    }

    public override void showAttackableTiles()
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            int distX = tile.coordinates.x - position.x;
            int distZ = tile.coordinates.z - position.z;
            int dist = distX + distZ;
            if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 <= skillDistance) //为了严谨，覆写。这里不要用攻击范围，而是用技能范围
            {
                tile.HighlightTile();
            }
        }
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
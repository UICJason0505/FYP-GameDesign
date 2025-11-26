using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HexMath;
using static MovingObject;
public class Chess : MonoBehaviour
{
    public int number = 1;
    public Player player;
    public int apCost = 1;
    private string name;
    private string className;
    private int id;
    public int attackArea = 1;
    public Coordinates position;
    public HexTile[] tiles;
    public GameManager gameManager;
    public bool isInAttackMode = false;
    [Header("UI属性面板")]
    public string PlayerName;
    public UnitInfoPanelController panel;
    public MovingObject move;
    public Renderer rend;
    public HexTile currentTile;
    Chess selectedChess;
    Chess attacker;
    int damage;
    
    // ========= 嘲讽系统（所有棋子共享） ===========
    [HideInInspector] public Chess tauntTarget = null;
    [HideInInspector] public int tauntRemainTurns = 0;


    public void Init(string className, int id, Player owner)
    {
        var go = GameObject.Find("GameManager");
        gameManager = go.GetComponent<GameManager>();
        if (go != null) move = go.GetComponent<MovingObject>();
        this.className = className;
        this.id = id;
        this.player = owner;
        this.name = className + "_" + id;
        gameObject.name = this.name;
    }
    // Start is called before the first frame update
    //为实现继承修改为 protected virtual
    protected virtual void Start()
    {
        var go = GameObject.Find("GameManager");
        gameManager = go.GetComponent<GameManager>();
        if (go != null) move = go.GetComponent<MovingObject>();
        // 自动获取场景中的 UI 面板
        if (panel == null)
        {
            panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault(); ;
        }

        // 尝试稳健地获取 MovingObject，避免 move 为 null 导致按键无效或 NRE
        if (move == null)
        {
            move = FindObjectOfType<MovingObject>();
        }
    }

    // Update is called once per frame
    //为实现继承修改为 protected virtual
    protected virtual void Update()
    {
        PlayerName = player.playerName;
        if (gameManager == null)
        {
            var go = GameObject.Find("GameManager");
            if (go != null)
            {
                gameManager = go.GetComponent<GameManager>();
            }
        }
        if (gameManager == null) return;
        if (gameManager.attacker != null) attacker = gameManager.attacker.GetComponent<Chess>();
        if(gameManager != null && gameManager.attacker != null)
        {
            attacker.isInAttackMode = true;
        }
        //选中逻辑
        if (SelectionManager.selectedObj == null) return;
        if (SelectionManager.selectedObj != gameObject) return;
        //UI关闭
        if (panel != null && Input.GetMouseButtonDown(1)) panel.Hide();

        // 对 move 做空检查：如果没有 move 实例也允许切换（便于调试或不同实现）
        if (Input.GetKeyDown(KeyCode.X) && (move == null || move.currentState == ObjectState.Selected))
        {
            isInAttackMode = !isInAttackMode;
            if (isInAttackMode)
            {
                showAttackableTiles();
                if(gameManager.attacker == null) gameManager.attacker = this.gameObject;
                Debug.Log("player:!!!!" + gameManager.attacker);
            }
            else
            {
                if (attacker == null||attacker.player == null) return;
                gameManager.attacker.GetComponent<Chess>().ResetTiles();
                gameManager.attacker = null;
            }
        }
        if(attacker == null || attacker.player == null) return;

        //     嘲讽强制攻击系统       //
        if (attacker.tauntTarget != null)
        {
            // 检查嘲讽目标是否仍存活
            if (attacker.tauntTarget == null || attacker.tauntTarget.player == null)
            {
                attacker.tauntTarget = null;
                attacker.tauntRemainTurns = 0;
            }
            else
            {
                // 检查是否在攻击范围内
                int dist = HexMath.HexDistance(attacker.position, attacker.tauntTarget.position);

                if (dist <= attacker.attackArea)
                {
                    Debug.Log($"{attacker.name} 因嘲讽被迫攻击 {attacker.tauntTarget.name}");

                    int dmg = attacker.attack();
                    attacker.tauntTarget.defend(dmg, attacker, attacker.tauntTarget);

                    attacker.player.actionPoints -= attacker.apCost;

                    attacker.ResetTiles();
                    attacker.isInAttackMode = false;

                    attacker.tauntRemainTurns--;
                    if (attacker.tauntRemainTurns <= 0)
                        attacker.tauntTarget = null;

                    return; // 阻止手动选择攻击
                }
            }
        }

        if (attacker.isInAttackMode && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("选中目标：" + "123");
                if (!hit.collider.CompareTag("King"))
                {
                    if (attacker == null) return;
                    attacker.ResetTiles();
                    attacker.isInAttackMode = false;
                    attacker = null;
                    return;
                }
                selectedChess = hit.collider.GetComponent<Chess>();
                Debug.Log("选中目标：" + selectedChess.gameObject.name);
                Debug.Log("123 " + gameManager.attacker.GetComponent<Chess>().player.playerName);
            }
            else
            {
                if (attacker == null) return;
                attacker.ResetTiles();
                attacker.isInAttackMode = false;
                attacker = null;
                return;
            }
            if (selectedChess == attacker)
            {
                if (attacker == null) return;
                attacker.ResetTiles();
                attacker.isInAttackMode = false;
                attacker = null;
                return;
            } // 不攻击自己
            if (attacker.player == selectedChess.player)
            {
                if (attacker == null) return;
                attacker.ResetTiles();
                attacker.isInAttackMode = false;
                attacker = null;
                return;
            } // 不攻击友军
            if(currentTile == null)
            {
                attacker.ResetTiles();
                attacker.isInAttackMode = false;
                attacker = null;
                return;
            }
            if (attacker.player.HasEnoughActionPoints(attacker.apCost) && currentTile.attackable == true)
            {
                damage = attacker.attack();//结算攻击
                defend(damage, attacker, selectedChess);//结算伤害和血量
                player.actionPoints -= apCost;
            }
            if(attacker == null) return;
            attacker.ResetTiles();
            attacker.isInAttackMode = false;
            attacker = null;

        }
        return;
    }
    void OnMouseDown()//UI显示
    {
        if (SelectionManager.isAttackMode) return;
        SelectionManager.selectedObj = gameObject;
        if (panel != null)
        {
            panel.ShowUnit(gameObject.name, number);
        }
    }
    public void showAttackableTiles()//显示可攻击范围
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
    public void ResetTiles()//攻击范围颜色显示
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            tile.ResetTile();
        }
    }

    public virtual int attack()//结算攻击
    {
        return 0;
    }
    public virtual void defend(int demage, Chess attacker, Chess target)//结算伤害和血量（number）6个兵种
    {
        return;
    }
    //public void ExecuteAttack(Chess attacker, Chess target)
    //{
    //    int aBefore = attacker.number;
    //    int bBefore = target.number;

    //    attacker.number -= bBefore;
    //    target.number -= aBefore;

    //    Debug.Log($"{name} 攻击 {target.name}：我方减 {bBefore}，敌方减 {aBefore} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

    //    if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number); // 更新自己面板
    //    if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);
    //    if (attacker.number <= 0)
    //    {
    //        Destroy(attacker.gameObject);
    //        Debug.Log($"{name} 被击败！");
    //    }

    //    if (target.number <= 0)
    //    {
    //        Destroy(target.gameObject);
    //        Debug.Log($"{target.name} 被击败！");
    //    }
    //}

    public void CollectTileValue(HexTile tile)//收集格子值
    {
        int value = tile.GetTileValue();
        number += value;
    }

    // 为所有棋子统一更新 position（与 King 中的逻辑一致）
    void OnTriggerStay(Collider other) => UpdatePositionFromTile(other);

    private void UpdatePositionFromTile(Collider other)
    {
        if (other.GetComponent<HexTile>() == null) return;
        currentTile = other.GetComponent<HexTile>();
        if (currentTile == null) return;
        position = currentTile.coordinates;
    }

    // 接收到嘲讽时由嘲讽者或 GM 调用
    public void ReceiveTaunt(Chess taunter, int duration = 1)
    {
        tauntTarget = taunter;
        tauntRemainTurns = duration;
    }

}
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
    bool isAttacking;
    public int blood = 5;
    public TurnManager turnManager;
    protected bool inputLocked = false;
    public Transform modelRoot;

    // 在 Chess 类的字段区域添加
    protected Chess protector = null; // 记录谁在保护这个单位

    // ========== 动画相关 ==========
    public Animator animator;
    public enum Anime
    {
        Idle = 0,
        Attack = 1,
        Death = 2,
        Skill = 3
    };
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
        // 动画组件获取
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
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
        var M = FindObjectOfType<TurnManager>();
        turnManager = M.GetComponent<TurnManager>();
    }

    // Update is called once per frame
    //为实现继承修改为 protected virtual
    protected virtual void Update()
    {
        if(this.currentTile.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            if(this.gameObject.layer == LayerMask.NameToLayer("King"))
            {
                for (int i = 0; i < turnManager.players.Count; i++)
                {
                    if (turnManager.players[i] == this.player)
                    {
                        turnManager.players.RemoveAt(i);
                        break;
                    }
                }
                Chess[] chess = FindObjectsOfType<Chess>();
                for (int i = 0; i < chess.Length; i++)
                {
                    if (chess[i] != this && chess[i].player == this.player)
                    {
                        StartCoroutine(DieRoutine(chess[i]));
                    }
                }
                StartCoroutine(DieRoutine(this));
            }
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            ResetTiles();
            isInAttackMode = false;
        }
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
        if(attacker != null && (attacker.gameObject.layer == LayerMask.NameToLayer("King") || this.gameObject.layer == LayerMask.NameToLayer("King"))) {}
        else 
        { 
            if (attacker == null || attacker.player == null) return;
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
                    gameManager.attacker = null;
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
                gameManager.attacker = null;
                return;
            }
            if (selectedChess == attacker)
            {
                if (attacker == null) return;
                attacker.ResetTiles();
                attacker.isInAttackMode = false;
                attacker = null;
                gameManager.attacker = null;
                return;
            } // 不攻击自己
            if (attacker.player == selectedChess.player)
            {
                if (attacker == null) return;
                attacker.ResetTiles();
                attacker.isInAttackMode = false;
                attacker = null;
                gameManager.attacker = null;
                return;
            } // 不攻击友军
            if(currentTile == null)
            {
                attacker.ResetTiles();
                attacker.isInAttackMode = false;
                attacker = null;
                gameManager.attacker = null;
                return;
            }
            if (attacker.gameObject.layer == LayerMask.NameToLayer("King") && attacker.player.HasEnoughActionPoints(attacker.apCost) && currentTile.attackable == true)
            {
                attacker.player.actionPoints -= apCost;
                attacker.KingAttack(attacker, selectedChess);
                if (attacker == null) return;
                attacker.ResetTiles();
                attacker.isInAttackMode = false;
                attacker = null;
                gameManager.attacker = null;
                return;
            }
            if (selectedChess.gameObject.layer == LayerMask.NameToLayer("King") && attacker.player.HasEnoughActionPoints(attacker.apCost) && currentTile.attackable == true)
            {
                Debug.Log("King");
                attacker.player.actionPoints -= apCost;
                selectedChess.KingDefend(attacker, selectedChess);
                if (attacker == null) return;
                attacker.ResetTiles();
                attacker.isInAttackMode = false;
                attacker = null;
                gameManager.attacker = null;
                return;
            }
            if (attacker.player.HasEnoughActionPoints(attacker.apCost) && currentTile.attackable == true)
            {
                damage = attacker.attack();//结算攻击
                if (attacker is Cannoneer cannoneer)
                {
                    if (attacker.player.HasEnoughActionPoints(2) && currentTile.attackable == true)
                    {
                    // 如果是 Cannoneer，用范围攻击
                    cannoneer.AreaAttack(damage, attacker, selectedChess);
                    attacker.player.actionPoints -= 2; //多扣一点
                    attacker.ResetTiles();
                    attacker.isInAttackMode = false;
                    attacker = null;
                    gameManager.attacker = null;
                    }
                }
                else
                {
                    // 普通单体攻击本来用单个defend()，但现在考虑到盾卫，会在 TakeDamage 里处理援护逻辑
                    selectedChess.TakeDamage(damage, attacker);
                    attacker.player.actionPoints -= apCost;
                    attacker.ResetTiles();
                    attacker.isInAttackMode = false;
                    attacker = null;
                    gameManager.attacker = null;
                }
            }
        }

        // 自动朝向摄像机（常态）
        if (modelRoot != null && !isInAttackMode)
        {
            var cam = Camera.main;
            if (cam != null)
            {
                Vector3 targetPos = cam.transform.position;
                targetPos.y = modelRoot.position.y; // 保持水平，不仰视不低头
                modelRoot.LookAt(targetPos);
            }
        }

        return;
    }

    // 添加一个方法来设置保护者
    public void SetProtector(Chess protectorChess)
    {
        protector = protectorChess;
    }


    // 添加一个方法来清除保护者
    public void ClearProtector()
    {
        protector = null;
    }


    // 获取保护者
    public Chess GetProtector()
    {
        return protector;
    }


    public virtual void RefreshProtection()
    {
        // 默认单位不做任何事
    }


    // 
    public static void RefreshAllShieldGuards()
    {
        ShieldGuard[] guards = FindObjectsOfType<ShieldGuard>();
        foreach (var guard in guards)
        {
            guard.RefreshProtection();
        }
    }


    public virtual void KingAttack(Chess attacker, Chess target){}


    public virtual void KingDefend(Chess attacker, Chess target){}


    public virtual void CheckDeath(Chess unit){}


    public virtual void KingDeathCheck(){}


    void OnMouseDown()//UI显示
    {
        if (SelectionManager.isAttackMode) return;
        SelectionManager.selectedObj = gameObject;
        if (panel != null)
        {
            panel.ShowUnit(gameObject.name, number);
        }
    }


    public virtual void showAttackableTiles()//显示可攻击范围
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


    public virtual void ResetTiles()//攻击范围颜色显示
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
        //等待攻击动画播放后进行受伤动画
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        return;
    }


    // 援护机制的统一入口
    public void TakeDamage(int damage, Chess attacker)
    {
        // 通用规则：援护
        if (protector != null)
        {
            Debug.Log($"{protector.name} 援护 {name}");
            protector.TakeDamage(damage, attacker);
            return;
        }

        // 调用真正的受伤逻辑（子类）
        defend(damage, attacker, this);
    }


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


    public IEnumerator AttackRoutine(Chess unit)
    {
        Animator anim = unit.animator;
        isAttacking = true;
        anim.SetInteger("State", (int)Anime.Attack);
        yield return null;
        while (true)
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Attack") && info.normalizedTime >= 1f)
                break;
            yield return null;
        }
        anim.SetInteger("State", (int)Anime.Idle);
        isAttacking = false;
    }


    public IEnumerator DieRoutine(Chess unit)
    {
        // 切到死亡状态
        while (true)
        {
            if(isAttacking == true) yield return null;
            else break;
        }
        Animator anim = unit.animator;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        anim.SetInteger("State", (int)Anime.Death);
        while (!info.IsName("Death"))
        {
            yield return null;
            info = anim.GetCurrentAnimatorStateInfo(0);
        }
        while (info.normalizedTime < 1f)
        {
            yield return null;
            info = anim.GetCurrentAnimatorStateInfo(0);
        }

        // 清除占用状态
        if (unit.currentTile != null)
        {
            if (unit.currentTile.occupyingChess == unit)
            {
                unit.currentTile.isOccupied = false;
                unit.currentTile.occupyingChess = null;
                Debug.Log($"{unit.name} 清除占用状态: {unit.currentTile.name}");
            }
        }

        // 播完再销毁
        Destroy(unit.gameObject);
    }


    public IEnumerator SkillRoutine(Chess unit)
    {
        Animator anim = unit.animator;
        anim.SetInteger("State", (int)Anime.Skill);
        yield return null;
        while (true)
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Skill") && info.normalizedTime >= 1f)
                break;
            yield return null;
        }
        anim.SetInteger("State", (int)Anime.Idle);
    }
}
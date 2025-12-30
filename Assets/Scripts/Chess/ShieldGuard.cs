using UnityEngine;

public class ShieldGuard : Chess
{
    [Header("ShieldGuard Skill Settings")]
    public int guardCD = 2;
    public int currentCD = 0; // 当前冷却计数器
    private bool usedThisTurn = false;
    TurnManager turnManager;


    protected override void Start()
    {
        base.Start();
        var temp = FindObjectOfType<TurnManager>();
        turnManager = temp.GetComponent<TurnManager>();
        number = 3;            // 初始数值：3
        attackArea = 1;        // 攻击距离：1
        apCost = 1;
    }

    protected override void Update()
    {
        base.Update();

        // 释放援护
        if (Input.GetKeyDown(KeyCode.C) && SelectionManager.selectedObj == gameObject)
        {
            TryActivateGuard();
        }
    }

    void TryActivateGuard()
    {
        if (usedThisTurn)
        {
            Debug.Log("本回合已经使用过援护");
            return;
        }

        if (currentCD > 0)
        {
            Debug.Log($"援护冷却中，还剩 {currentCD} 回合");
            return;
        }

        ActivateGuard();
        usedThisTurn = true;
        currentCD = guardCD;
    }



    void ActivateGuard()
    {
        Chess[] allChess = FindObjectsOfType<Chess>();

        foreach (Chess unit in allChess)
        {
            if (unit == this) continue;
            if (unit.player != this.player) continue;

            // 六边形距离 1
            if (HexMath.HexDistance(unit.position, this.position) == 1)
            {
                unit.SetProtector(this);
                Debug.Log($"{name} 正在援护 {unit.name}");
            }
        }

        StartCoroutine(SkillRoutine(this));
    }


    public void OnTurnStart(Player currentPlayer)
    {
        if (currentPlayer != this.player) return;

        usedThisTurn = false;

        if (currentCD > 0)
            currentCD--;
    }


    // 重写攻击受伤
    // 盾卫造成伤害减半
    public override int attack()
    {
        StartCoroutine(AttackRoutine(this));    
        int dmg = Mathf.Max(1, number / 2);
        Debug.Log($"{name} 发动攻击（伤害减半）：{dmg}");
        return dmg;
    }

    public override void defend(int damage, Chess attacker, Chess target)
    {
        // 1. 盾卫受到的伤害减半
        int actualDamage = Mathf.Max(1, damage / 2);
        number -= actualDamage;

        Debug.Log($"{name} 承受伤害（减半）：{actualDamage} 剩余 {number}");

        // 2. 反击
        if (HexMath.HexDistance(attacker.position, this.position) == 1)
        {
            int retaliateDamage = Mathf.Max(1, number / 2);
            attacker.number -= retaliateDamage;
            StartCoroutine(AttackRoutine(target));
            Debug.Log($"{name} 对 {attacker.name} 触发反击！造成 {retaliateDamage}");

        }
        if (number <= 0) StartCoroutine(DieRoutine(target));
        if (attacker.number <= 0) StartCoroutine(DieRoutine(attacker));
    }
}

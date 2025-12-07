using UnityEngine;

public class ShieldGuard : Chess
{
    [Header("ShieldGuard Skill Settings")]
    public int tauntCD = 2;
    private int tauntTimer = 0;
    public int tauntRange = 1;
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
        if (tauntTimer > 0) tauntTimer--;

        // 释放嘲讽
        if (Input.GetKeyDown(KeyCode.C) && SelectionManager.selectedObj == gameObject)
        {
            TryTaunt();
        }
    }

    // 嘲讽技能
    void TryTaunt()
    {
        if (tauntTimer > 0)
        {
            Debug.Log("嘲讽技能冷却中");
            return;
        }

        Debug.Log($"{name} 释放嘲讽！");
        tauntTimer = tauntCD;

        BroadcastTaunt();
    }

    void BroadcastTaunt()
    {
        foreach (Chess c in FindObjectsOfType<Chess>())
        {
            if (c == this) continue;
            if (c.player == this.player) continue;

            int dist = HexMath.HexDistance(c.position, this.position);
            if (dist <= tauntRange)
            {
                c.GetComponent<ITauntable>()?.ReceiveTaunt(this);
            }
        }
    }

    // 重写攻击受伤

    // 盾卫造成伤害减半
    public override int attack()
    {
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


        // 2. 死亡检查
        if (number <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // 3. 反击
        if (HexMath.HexDistance(attacker.position, this.position) == 1)
        {
            int retaliateDamage = Mathf.Max(1, number / 2);
            attacker.number -= retaliateDamage;

            Debug.Log($"{name} 对 {attacker.name} 触发反击！造成 {retaliateDamage}");

            if (attacker.number <= 0)
                Destroy(attacker.gameObject);
        }
    }
}

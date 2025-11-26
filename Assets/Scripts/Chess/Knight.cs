using UnityEngine;

public class Knight : Chess
{
    [Header("Knight Skill Settings")]
    public int dashCD = 2;
    private int dashTimer = 0;

    protected override void Start()
    {
        base.Start();
        number = 1;
        attackArea = 1;
        apCost = 1;
    }

    protected override void Update()
    {
        base.Update();
        if (dashTimer > 0) dashTimer--;

        // 主动技能：冲刺
        if (Input.GetKeyDown(KeyCode.C) && SelectionManager.selectedObj == gameObject)
        {
            TryDash();
        }
    }

    // 冲刺技能
    void TryDash()
    {
        if (dashTimer > 0)
        {
            Debug.Log("冲刺冷却中");
            return;
        }

        Chess target = TryGetTargetInFront();
        if (target == null)
        {
            Debug.Log("前方没有敌人，无法冲刺");
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector3 pushPos = target.transform.position + dir * 1.1f;

        // 检查 push 位置是否为空
        if (!IsTileEmpty(pushPos))
        {
            Debug.Log("目标身后被堵住，无法击退");
            return;
        }

        // 击退
        target.transform.position = pushPos;

        dashTimer = dashCD;
        Debug.Log($"{name} 冲刺并击退 {target.name}！");
    }

    Chess TryGetTargetInFront()
    {
        foreach (Chess c in FindObjectsOfType<Chess>())
        {
            if (c == this) continue;
            if (c.player == this.player) continue;
            if (HexMath.HexDistance(c.position, this.position) == 1)
                return c;
        }
        return null;
    }

    bool IsTileEmpty(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, 0.3f);
        foreach (var h in hits)
            if (h.CompareTag("Chess")) return false;
        return true;
    }

    // 重写攻击/反伤
    public override int attack()
    {
        return number;
    }

    public override void defend(int damage, Chess attacker, Chess target)
    {
        number -= damage;
        Debug.Log($"{name} 受到伤害：{damage} 剩余 {number}");

        if (number <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (HexMath.HexDistance(attacker.position, this.position) == 1)
        {
            attacker.number -= number;
            Debug.Log($"{name} 反击 {attacker.name}，造成 {number} 伤害");
            if (attacker.number <= 0)
                Destroy(attacker.gameObject);
        }
    }

}

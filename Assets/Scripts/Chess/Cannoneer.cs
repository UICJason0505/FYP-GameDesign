using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using static MovingObject;

public class Cannoneer: Chess
{
    [Header("Archer属性")]
    private int initialValue = 1;
    private int attackDistance = 3; // 2~3
    TurnManager turnManager;
    // Awake 用于获取全局引用，避免 move 为 null
    private static readonly HexMath.Coordinates[] hexDirections =
    {
        new HexMath.Coordinates(+1, 0),
        new HexMath.Coordinates(+1, -1),
        new HexMath.Coordinates(0, -1),
        new HexMath.Coordinates(-1, 0),
        new HexMath.Coordinates(-1, +1),
        new HexMath.Coordinates(0, +1),
    };

    private void Awake()
    {
        var go = GameObject.Find("GameManager");
        if (go != null) move = go.GetComponent<MovingObject>();
        if (panel == null)
        {
            panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault();
        }
        var temp = FindObjectOfType<TurnManager>();
        turnManager = temp.GetComponent<TurnManager>();
        //添加 TurnManager 初始化
    }

    protected override void Start()
    {
        number = initialValue;
        attackArea = attackDistance;
        if (this.gameObject.name == "Cannoneer")
        {
            player = turnManager.players[0];
        }
        else
        {
            player = turnManager.players[1];
        }
    }
    public override int attack()
    {
        return this.number;
    }

    private void SplashDamage(Chess mainTarget, int splashDamage)
    {
        float radius = HexTile.radius; // Hex 半径

        // 取得主目标的六边形坐标
        HexMath.Coordinates center = HexMath.WorldToCoordinates(mainTarget.transform.position, radius);

        // 遍历六向邻居
        foreach (var dir in hexDirections)
        {
            HexMath.Coordinates neighbor = new HexMath.Coordinates(center.x + dir.x, center.z + dir.z);

            // 转世界坐标
            Vector3 pos = HexMath.CoordinatesToWorld(neighbor, radius);

            // 找格子内的单位
            Collider[] hits = Physics.OverlapSphere(pos, 0.3f);

            foreach (var h in hits)
            {
                Chess other = h.GetComponent<Chess>();

                // 排除主目标，不分敌我都能被溅射（如果你要排除队友可以告诉我）
                if (other != null && other != mainTarget)
                {
                    other.number -= splashDamage;

                    Debug.Log($"Cannoneer 溅射命中 {other.name}，伤害 {splashDamage} 剩余 {other.number}");

                    if (panel != null) panel.ShowUnit(other.name, other.number);

                    if (other.number <= 0)
                    {
                        Destroy(other.gameObject);
                        Debug.Log($"{other.name} 被溅射击败！");
                    }
                }
            }
        }
    }

    public override void defend(int damage, Chess attacker, Chess target)
    {
        if (attacker.gameObject.layer == LayerMask.NameToLayer("Saber"))
        {
            int aBefore = attacker.number;
            int bBefore = target.number;

            target.number -= damage;

            Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

            if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number); // 更新自己面板
            if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);
            if (attacker.number <= 0)
            {
                Destroy(attacker.gameObject);
                Debug.Log($"{name} 被击败！");
            }

            if (target.number <= 0)
            {
                Destroy(target.gameObject);
                Debug.Log($"{target.name} 被击败！");
            }
        }
        else if (attacker.gameObject.layer == LayerMask.NameToLayer("Archer"))
        {
            int aBefore = attacker.number;
            int bBefore = target.number;
            target.number -= damage;

            Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

            if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);

            if (target.number <= 0)
            {
                Destroy(target.gameObject);
                Debug.Log($"{target.name} 被击败！");
            }
        }
        else if (attacker.gameObject.layer == LayerMask.NameToLayer("ShieldGuard"))
        {
            int aBefore = attacker.number;
            int bBefore = target.number;

            target.number -= damage;

            Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

            if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number); // 更新自己面板
            if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);
            if (attacker.number <= 0)
            {
                Destroy(attacker.gameObject);
                Debug.Log($"{name} 被击败！");
            }

            if (target.number <= 0)
            {
                Destroy(target.gameObject);
                Debug.Log($"{target.name} 被击败！");
            }
        }
        else if (attacker.gameObject.layer == LayerMask.NameToLayer("Knight"))
        {
            int aBefore = attacker.number;
            int bBefore = target.number;

            target.number -= damage;

            Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

            if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number); // 更新自己面板
            if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);
            if (attacker.number <= 0)
            {
                Destroy(attacker.gameObject);
                Debug.Log($"{name} 被击败！");
            }

            if (target.number <= 0)
            {
                Destroy(target.gameObject);
                Debug.Log($"{target.name} 被击败！");
            }
        }
        else if (attacker.gameObject.layer == LayerMask.NameToLayer("Cannoneer"))
        {
            int aBefore = attacker.number;
            int bBefore = target.number;
            target.number -= damage;

            Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

            if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);

            if (target.number <= 0)
            {
                Destroy(target.gameObject);
                Debug.Log($"{target.name} 被击败！");
            }
        }
        else if (attacker.gameObject.layer == LayerMask.NameToLayer("Peasant"))
        {
            int aBefore = attacker.number;
            int bBefore = target.number;


            target.number -= damage;

            Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

            if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number); // 更新自己面板
            if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);
            if (attacker.number <= 0)
            {
                Destroy(attacker.gameObject);
                Debug.Log($"{name} 被击败！");
            }

            if (target.number <= 0)
            {
                Destroy(target.gameObject);
                Debug.Log($"{target.name} 被击败！");
            }
        }
    }
}
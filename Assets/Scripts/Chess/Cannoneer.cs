using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using static MovingObject;

public class Cannoneer : Chess
{
    [Header("Cannoneer Attribute")]
    private int initialValue = 1; // 初始数值
    private int attackDistance = 3; // 攻击距离 2~3
    TurnManager turnManager; // Awake 用于获取全局引用，避免 move 为 null

    // 格子半径：根据地图实际 tile 半径设置（默认为 1）
    [SerializeField] private float hexRadius = 1f;

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
        if (this.gameObject.name == "Archer")
        {
            player = turnManager.players[0]; // 玩家1
        }
        else
        {
            player = turnManager.players[1]; // 玩家2
        }
    }

    // 攻击方法：如果在范围内，对所有敌方单位造成伤害（群体伤害）
    public override int attack()
    {
        int damage = this.number;

        // 当前单位的六边形坐标（通过世界坐标转换）
        var myCoord = HexMath.WorldToCoordinates(this.transform.position, hexRadius);

        // 遍历场上所有单位，命中在范围内的敌方
        var allChesses = FindObjectsOfType<Chess>();
        foreach (var other in allChesses)
        {
            if (other == this) continue;
            if (other.player == this.player) continue; // 只攻击敌方

            var otherCoord = HexMath.WorldToCoordinates(other.transform.position, hexRadius);
            int dist = HexMath.HexDistance(myCoord, otherCoord);
            if (dist <= attackArea)
            {
                other.defend(damage, this, other);
            }
        }

        return damage;
    }

    public override void defend(int damage, Chess attacker, Chess target)
    {
        int attackerLayer = attacker.gameObject.layer;

        switch (attackerLayer)
        {
            // 反击派（近战）：Saber、ShieldGuard、Knight、Peasant
            case int layer when layer == LayerMask.NameToLayer("Saber"):
            case int layer2 when layer2 == LayerMask.NameToLayer("ShieldGuard"):
            case int layer3 when layer3 == LayerMask.NameToLayer("Knight"):
            case int layer4 when layer4 == LayerMask.NameToLayer("Peasant"):
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    attacker.number -= bBefore;
                    target.number -= aBefore;

                    Debug.Log($"{attacker.name} 攻击 {target.name}：我方减 {bBefore}，敌方减 {aBefore} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

                    if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number);
                    if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);

                    if (attacker.number <= 0)
                    {
                        Destroy(attacker.gameObject);
                        Debug.Log($"{attacker.name} 被击败！");
                    }
                    if (target.number <= 0)
                    {
                        Destroy(target.gameObject);
                        Debug.Log($"{target.name} 被击败！");
                    }
                    break;
                }

            // 不反击派（远程）：Archer、Cannoneer
            case int layer5 when layer5 == LayerMask.NameToLayer("Archer"):
            case int layer6 when layer6 == LayerMask.NameToLayer("Cannoneer"):
                {
                    target.number -= damage;
                    Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage}");

                    if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);

                    if (target.number <= 0)
                    {
                        Destroy(target.gameObject);
                        Debug.Log($"{target.name} 被击败！");
                    }
                    break;
                }

            default:
                Debug.LogWarning("未知攻击单位 Layer：" + attackerLayer);
                break;
        }
    }

}
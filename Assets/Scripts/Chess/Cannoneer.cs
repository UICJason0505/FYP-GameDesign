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

    // 攻击方法：返回当前生命值作为攻击力
    public override int attack()
    {
        return this.number;
    }

    public override void defend(int damage, Chess attacker, Chess target)
    {
        int attackerLayer = attacker.gameObject.layer;

        switch (attackerLayer)
        {
            // Saber（剑士）
            case int layer when layer == LayerMask.NameToLayer("Saber"):
                {
                    // int aBefore = attacker.number;
                    // int bBefore = target.number;
                    target.number -= damage;

                    Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

                    if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number);
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
                    break;
                }

            // Archer（弓箭手 - 远程，不受反击）
            case int layer when layer == LayerMask.NameToLayer("Archer"):
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

            // ShieldGuard（盾卫）
            case int layer when layer == LayerMask.NameToLayer("ShieldGuard"):
            // Knight（骑士）
            case int layer2 when layer2 == LayerMask.NameToLayer("Knight"):
            // Peasant（农民）
            case int layer3 when layer3 == LayerMask.NameToLayer("Peasant"):
                {
                    target.number -= damage;

                    Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage}");

                    if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number);
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
                    break;
                }

            // Cannoneer（炮手 - 远程，不受反击）
            case int layer when layer == LayerMask.NameToLayer("Cannoneer"):
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using static MovingObject;

public class Archer : Chess
{
    [Header("Archer属性")]
    private int initialValue = 3;
    private int attackDistance = 1;
    TurnManager turnManager;
    // Awake 用于获取全局引用，避免 move 为 null
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

    private void Start()
    {
        number = initialValue;
        attackArea = attackDistance;
        if(this.gameObject.name == "Archer")
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
            int distX = Mathf.Abs(attacker.position.x - target.position.x);
            int distZ = Mathf.Abs(attacker.position.z - target.position.z);
            int dist = distX + distZ;
            if (dist > 1)
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
            else
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
        else if (attacker.gameObject.layer == LayerMask.NameToLayer("Gunner"))
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
        else if (attacker.gameObject.layer == LayerMask.NameToLayer("Farmer"))
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
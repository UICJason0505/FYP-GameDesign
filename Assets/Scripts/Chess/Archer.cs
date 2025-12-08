using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using static MovingObject;

public class Archer : Chess
{
    [Header("Archer属性")]
    private int initialValue = 2;
    private int attackDistance = 2;
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
    }
    public override int attack()
    {
        StartCoroutine(AttackRoutine(this));
        return this.number;
    }
    public override void defend(int damage, Chess attacker, Chess target)
    {
        int aBefore = attacker.number;
        int bBefore = target.number;

        target.number -= damage;

        Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

        if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number); // 更新自己面板
        if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);
        if (attacker.number <= 0)
        {
            StartCoroutine(DieRoutine(attacker));
            Debug.Log($"{name} 被击败！");
        }

        if (target.number <= 0)
        {
            StartCoroutine(DieRoutine(target)); 
            Debug.Log($"{target.name} 被击败！");
        }
    }
}
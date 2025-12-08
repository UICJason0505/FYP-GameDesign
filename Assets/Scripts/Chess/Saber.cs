using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using static Chess;
using static MovingObject;

public class Saber : Chess
{
    [Header("Saber属性")]
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
        animator.SetInteger("State", (int)Anime.Idle);
        Debug.Log(animator.GetInteger("State"));
    }
    public override int attack()
    {
        StartCoroutine(AttackRoutine(this));
        return this.number;
    }
    public override void defend(int damage, Chess attacker, Chess target)
    {
        int layer = attacker.gameObject.layer;
        int saberLayer = LayerMask.NameToLayer("Saber");
        int knightLayer = LayerMask.NameToLayer("Knight");
        int peasantLayer = LayerMask.NameToLayer("Peasant");
        int archerLayer = LayerMask.NameToLayer("Archer");
        int shielderLayer = LayerMask.NameToLayer("ShieldGuard");
        int cannoneerLayer = LayerMask.NameToLayer("Cannoneer");
        switch (layer)
        {
            // 近战普攻：Saber / Knight / Peasant / Shielder—— 完全相同逻辑，合并
            case int _ when layer == saberLayer || layer == knightLayer || layer == peasantLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    attacker.number -= number;
                    target.number -= damage;
                    StartCoroutine(AttackRoutine(target));
                    // 更新面板
                    if (panel != null)
                    {
                        panel.ShowUnit(attacker.gameObject.name, attacker.number);
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    // 判定死亡
                    if (attacker.number <= 0)
                    {
                        StartCoroutine(DieRoutine(attacker));
                        Debug.Log($"{attacker.name} 被击败！");
                    }
                    if (target.number <= 0)
                    {
                        StartCoroutine(DieRoutine(target));
                        Debug.Log($"{target.name} 被击败！");
                    }
                    break;
                }

            case int _ when layer == cannoneerLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    target.number -= damage;

                    if (panel != null)
                    {
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    if (target.number <= 0)
                    {
                        StartCoroutine(DieRoutine(target));
                        Debug.Log($"{target.name} 被击败！");
                    }
                    break;
                }
            case int _ when layer == archerLayer:
                {
                    int distX = attacker.position.x - target.position.x;
                    int distZ = attacker.position.z - target.position.z;
                    int dist = distX + distZ;
                    if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 <= 1)
                    {
                        int aBefore = attacker.number;
                        int bBefore = target.number;
                        StartCoroutine(AttackRoutine(target));
                        attacker.number -= bBefore;
                        target.number -= damage;

                        Debug.Log($"{attacker.name} 攻击 {target.name}：我方减 {bBefore}，敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

                        if (panel != null) panel.ShowUnit(attacker.gameObject.name, attacker.number); // 更新自己面板
                        if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);
                        if (attacker.number <= 0)
                        {
                            StartCoroutine(DieRoutine(attacker));
                            Debug.Log($"{attacker.name} 被击败！");
                        }
                        if (target.number <= 0)
                        {
                            StartCoroutine(DieRoutine(target));
                            Debug.Log($"{target.name} 被击败！");
                        }
                    }
                    else if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 <= 2)
                    {
                        int aBefore = attacker.number;
                        int bBefore = target.number;
                        target.number -= damage;

                        Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

                        if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);

                        if (target.number <= 0)
                        {
                            StartCoroutine(DieRoutine(target));
                            Debug.Log($"{target.name} 被击败！");
                        }
                    }
                    break;
                }
            case int _ when layer == shielderLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    attacker.number -= (int)(number * 0.5);
                    target.number -= damage;
                    StartCoroutine(AttackRoutine(target));
                    // 更新面板
                    if (panel != null)
                    {
                        panel.ShowUnit(attacker.gameObject.name, attacker.number);
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    // 判定死亡
                    if (attacker.number <= 0)
                    {
                        StartCoroutine(DieRoutine(attacker));
                        Debug.Log($"{attacker.name} 被击败！");
                    }
                    if (target.number <= 0)
                    {
                        StartCoroutine(DieRoutine(target));
                        Debug.Log($"{target.name} 被击败！");
                    }
                    break;
                }
        }
    }
}
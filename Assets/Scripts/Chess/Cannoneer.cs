using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using static HexMath;
using static HexTile;

public class Cannoneer : Chess
{
    [Header("Cannoneer属性")]
    private int initialValue = 1;
    private int attackDistance = 3; // 2~3
    TurnManager turnManager;

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
    }

    protected override void Start()
    {
        number = initialValue;
        attackArea = attackDistance;
        if (this.gameObject.name == "Cannoneer") //假定阵营
        {
            player = turnManager.players[0];
        }
        else
        {
            player = turnManager.players[1];
        }
    }

    private static readonly HexMath.Coordinates[] directions = new HexMath.Coordinates[]
    {
        new HexMath.Coordinates(+1,  0),
        new HexMath.Coordinates(+1, -1),
        new HexMath.Coordinates( 0, -1),
        new HexMath.Coordinates(-1,  0),
        new HexMath.Coordinates(-1, +1),
        new HexMath.Coordinates( 0, +1),
    };

    public void AreaAttack(int damage, Chess attacker, Chess target)
    {
        // 1. 先攻击中心（target）
        target.defend(damage, attacker, target);
        StartCoroutine(AttackRoutine(this));    
        // 2. 再攻击六方向的邻居
        foreach (var dir in directions)
        {
            // 计算邻居坐标
            HexMath.Coordinates neighborCoor = new HexMath.Coordinates(
                target.position.x + dir.x,
                target.position.z + dir.z
            );

            // 找到该坐标上是否有棋子
            Chess neighbor = HexTile.GetChessAt(neighborCoor);
            if (neighbor == null) continue;

            // 不攻击己方（可选）
            if (neighbor.player == attacker.player) continue;
            neighbor.defend(damage, attacker, neighbor);
        }
    }


    public override int attack()
    {
        return this.number;
    }

    // Cannoneer 作为target的情况
    public override void defend(int damage, Chess attacker, Chess target)
    {
        target.number -= damage;
        Debug.Log($"{attacker.name} 攻击 {target.name}：敌方减 {damage} 我方剩余血量{attacker.number} 敌方剩余血量{target.number}");

        if (panel != null) panel.ShowUnit(target.gameObject.name, target.number);
        if (target.number <= 0)
        {
            StartCoroutine(DieRoutine(target));
            Debug.Log($"{target.name} 被击败！");
            HexTile.RefreshAllChess();
        }
    }
}
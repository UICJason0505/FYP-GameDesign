using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;
using static HexMath;
using static MovingObject;
using static UnityEditor.FilePathAttribute;
//private bool isSelected = false;
public class King : Chess
{
    [Header("基础属性")]
    public TurnManager turnManager;
    private bool isSummonMode = false;
    int attackPower;
    public void Awake()
    {
        var go = GameObject.Find("GameManager");           
        if (go != null) move = go.GetComponent<MovingObject>();
        if (panel == null)
        {
            panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault(); ;
        }

        // 尝试稳健地获取 MovingObject，避免 move 为 null 导致按键无效或 NRE
        if (move == null)
        {
            move = FindObjectOfType<MovingObject>();
        }
        panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault();
        turnManager = FindObjectOfType<TurnManager>();
        attackPower = int.MaxValue;
    }
    public void Start()
    {
        number = 5;
    }
    void OnMouseDown()
    {
        if (panel != null)
        {
            panel.ShowUnit(player.playerName, number);
        }
    }

    public override void KingAttack(Chess attacker, Chess target)
    {
        int layer = target.gameObject.layer;
        int saberLayer = LayerMask.NameToLayer("Saber");
        int knightLayer = LayerMask.NameToLayer("Knight");
        int peasantLayer = LayerMask.NameToLayer("Peasant");
        int archerLayer = LayerMask.NameToLayer("Archer");
        int shielderLayer = LayerMask.NameToLayer("ShieldGuard");
        int cannoneerLayer = LayerMask.NameToLayer("Cannoneer");
        int kingLayer = LayerMask.NameToLayer("King");
        switch (layer)
        {
            // 近战普攻：Saber / Knight / Peasant / Shielder—— 完全相同逻辑，合并
            case int _ when layer == saberLayer || layer == knightLayer || layer == peasantLayer|| layer == shielderLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    attacker.number --;
                    target.number -= attackPower;
                    // 更新面板
                    if (panel != null)
                    {
                        panel.ShowUnit(attacker.gameObject.name, attacker.number);
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    // 判定死亡
                    KingDeathCheck();
                    CheckDeath(target);
                    break;
                }

            case int _ when layer == cannoneerLayer || layer == archerLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;

                    target.number -= attackPower;

                    if (panel != null)
                    {
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    CheckDeath(target);
                    break;
                }
            case int _ when layer == kingLayer:
                {
                    int aBefore = attacker.number;
                    int bBefore = target.number;
                    attacker.number--;
                    target.number --;
                    // 更新面板
                    if (panel != null)
                    {
                        panel.ShowUnit(attacker.gameObject.name, attacker.number);
                        panel.ShowUnit(target.gameObject.name, target.number);
                    }

                    // 判定死亡
                    KingDeathCheck();
                    target.KingDeathCheck();
                    break;
                }
        }
    }

    // 统一处理死亡
    public override void  CheckDeath(Chess unit)
    {
        if (unit.number <= 0)
        {
            Destroy(unit.gameObject);
        }
    }
    public override void KingDeathCheck()
    {
        if (this.number <= 0)
        {
            Destroy(this.gameObject);
            for(int i = 0; i < turnManager.players.Count; i++)
            {
                if(turnManager.players[i] == this.player)
                {
                    turnManager.players.RemoveAt(i);
                    break;
                }
            }
        }
    }
    public override void KingDefend(Chess attacker, Chess target)
    {
        print("King Defend called");
        int aBefore = attacker.number;
        int bBefore = target.number;

       attacker.number -= attackPower;
       target.number --;

        // 更新面板
        if (panel != null)
        {
            panel.ShowUnit(attacker.gameObject.name, attacker.number);
            panel.ShowUnit(target.gameObject.name, target.number);
        }

        // 判定死亡
        target.KingDeathCheck();
        CheckDeath(attacker);
           
    }
    public void Summon()
    {

    }
    public void ShowSummonArea()
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            int distX = tile.coordinates.x - position.x;
            int distZ = tile.coordinates.z - position.z;
            int dist = distX + distZ;
            if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 <= attackArea)
            {
                bool occupied = false;
                for (int j = 0; j < gameManager.occupiedCoord.Count; j++)
                {
                    if(tile.coordinates.x == gameManager.occupiedCoord[j].x && tile.coordinates.y == gameManager.occupiedCoord[j].y)
                    {
                        occupied = true;
                        break;
                    }
                }
                if(occupied == false) tile.HighlightTile();
            }
        }
    }
    private void Die()
    {
    }
    void OnTriggerStay(Collider other) => UpdatePositionFromTile(other);

    private void UpdatePositionFromTile(Collider other)
    {
        HexTile tile = other.GetComponent<HexTile>();
        if (tile == null) return;            
        position = tile.coordinates;         
    }
}

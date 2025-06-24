using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static HexMath;
//private bool isSelected = false;
public class King : MonoBehaviour
{
    [Header("基础属性")]
    public string unitName = "King";
    public int blood = 5;
    public UnitInfoPanelController panel;
    public TurnManager turnManager;
    private bool isSummonMode = false;
    public bool isInAttackMode = false;
    public Coordinates position;
    private int attackArea = 1;
    void Update()
    {
        if (MovingObject.selectedObj != gameObject) return;

        // ✅ 按 C 启动/退出召唤模式
        if (Input.GetKeyDown(KeyCode.C))
        {
            isSummonMode = !isSummonMode;
            Debug.Log("召唤模式切换：" + (isSummonMode ? "开启" : "关闭"));
        }

        // ✅ 按右键取消选择
        if (panel != null && Input.GetMouseButtonDown(1))
        {
            panel.Hide();
            MovingObject.selectedObj = null;
            isSummonMode = false; // 退出召唤模式
        }

        // ✅ 只有在召唤模式下才允许召唤
        if (!isSummonMode) return;

        Player player = turnManager.players[turnManager.turnCount];

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player.HasEnoughActionPoints(2))
            {
                Debug.Log("召唤近战棋子");
                GridBuildingSystem.Instance.SpawnChess1();
                player.UseActionPoint(2);
            }
            else
            {
                Debug.Log("行动点不足，不能召唤近战棋子");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (player.HasEnoughActionPoints(2))
            {
                Debug.Log("召唤远程棋子");
                GridBuildingSystem.Instance.SpawnChess2();
                player.UseActionPoint(2);
            }
            else
            {
                Debug.Log("行动点不足，不能召唤远程棋子");
            }
            if (MovingObject.selectedObj == null) return;
        }
        if (Input.GetKeyDown(KeyCode.X) && isInAttackMode == false)
        {
            showAttackableTiles();
            isInAttackMode = true;
            return;
        }
        if (Input.GetKeyDown(KeyCode.X) && isInAttackMode == true)
        {
            ResetTiles();
            isInAttackMode = false;
        }
    }
    void OnMouseDown()
    {
        if (panel != null)
        {
            panel.ShowUnit(unitName, blood);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chess")) TakeDamage();
    }

    public void TakeDamage()
    {
        blood--;
        if (blood <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.GameOver("King is dead! Game Over.");
        Destroy(gameObject);
    }
    public void showAttackableTiles()
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            int distX = tile.coordinates.x - position.x;
            int distZ = tile.coordinates.z - position.z;
            int dist = distX + distZ;
            if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 <= attackArea)
            {
                tile.HighlightTile();
            }
        }
    }
    public void ResetTiles()
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            tile.ResetTile();
        }
    }
    void OnTriggerStay(Collider other) => UpdatePositionFromTile(other);

    private void UpdatePositionFromTile(Collider other)
    {
        HexTile tile = other.GetComponent<HexTile>();
        if (tile == null) return;            
        position = tile.coordinates;         
    }
}

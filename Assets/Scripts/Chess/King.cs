using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour
{
    [Header("基础属性")]
    public string unitName = "King";
    public int blood = 5;
    public UnitInfoPanelController panel;
    public TurnManager turnManager;
    private bool isSummonMode = false;

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
}

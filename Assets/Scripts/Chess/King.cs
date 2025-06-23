using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour
{
    [Header("基础属性")]
    public string unitName = "King";
    public int blood = 5;
    public UnitInfoPanelController panel;
    void Update()
    {
        if (panel != null && Input.GetMouseButtonDown(1))
        {
            panel.Hide();
        }
        //if (!isSelected) return; 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GridBuildingSystem.Instance.SpawnChess1();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GridBuildingSystem.Instance.SpawnChess2();
        }
        /*

        var player = TurnManager.Instance.players[TurnManager.Instance.CurrentPlayerIndex];

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player.HasEnoughActionPoints(2))
            {
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
                GridBuildingSystem.Instance.SpawnChess2();
                player.UseActionPoint(2);
            }
            else
            {
                Debug.Log("行动点不足，不能召唤远程棋子");
            }
        }*/
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

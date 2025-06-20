using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour
{
    [Header("基础属性")]
    public string unitName = "国王";
    public int blood = 5;
    public UnitInfoPanelController panel;

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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MovingObject;

public class Archer : Chess
{
    [Header("Saber属性")]
    private int initialValue = 3;
    private int attackDistance = 1;
    // Awake 用于获取全局引用，避免 move 为 null
    private void Awake()
    {
        var go = GameObject.Find("GameManager");
        if (go != null) move = go.GetComponent<MovingObject>();
        if (panel == null)
        {
            panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault();
        }

        //添加 TurnManager 初始化
    }

    private void Start()
    {
        number = initialValue;
        attackArea = attackDistance;
    }


    
}
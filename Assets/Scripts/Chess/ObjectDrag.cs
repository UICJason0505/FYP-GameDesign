using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{   
    /*
    // 记录点击瞬间的相对位移
    private Vector3 mouseOffset;

    private void OnMouseDown()
    {
        mouseOffset = transform.position - GridBuildingSystem.GetMouseWorldPosition();
    }

    private void Update()
    {
        Vector3 pos = GridBuildingSystem.GetMouseWorldPosition() + mouseOffset;
        // 调用修正吸附函数，吸附到网格中
        transform.position = GridBuildingSystem.instance.SnapCoordinateToGrid(pos);
    }
    */
}

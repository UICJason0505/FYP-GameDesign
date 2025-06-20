using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    private GameObject selectedObj;

    void Start(){}

    void Update()
    {
        // 左键高亮
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObj == null)
            {
                // 执行射线检测
                RaycastHit hit = CastRay();
                if (!hit.collider) { return; } // 如果没有检测到物体，则返回

                // 获取到点击的物体
                selectedObj = hit.collider.gameObject;

                // 直接启用物体的 Outline 组件，显示高亮
                Outline outline = selectedObj.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = true;
                }
            }
        }

        // 右键点击取消高亮
        if (Input.GetMouseButtonDown(1))
        {
            // 执行射线检测
            RaycastHit hit = CastRay();
            if (!hit.collider) { return; } // 如果没有检测到物体，则返回

            // 获取到点击的物体
            GameObject clickedObject = hit.collider.gameObject;

            // 取消物体的高亮效果
            Outline outline = clickedObject.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false; // 禁用轮廓
            }
            selectedObj = null; // 清空当前选中的物体
        }
    }

    // 射线检测
    private RaycastHit CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从鼠标位置发射射线

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) // 检测射线是否击中物体
        {
            return hit;
        }

        return default;
    }
}


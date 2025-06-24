using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static GameObject selectedObj;  // 当前选中的物体
    public static bool isObjectSelected = false;  // 标记物体是否已选中

    // 每帧更新检查鼠标左键点击事件
    void Update()
    {
        // 如果鼠标左键按下，选中物体
        if (Input.GetMouseButtonDown(0))
        {
            // 执行射线检测，获取点击的物体
            GameObject clickedObject = RayToGetObj();
            if (clickedObject == null) return;

            // 检查是否有PlacebleObject脚本
            PlacebleObject placebleObject = clickedObject.GetComponent<PlacebleObject>();
            if (placebleObject == null) return;

            SelectObject(clickedObject);
        }

        // 右键点击取消高亮并取消选中物体
        if (Input.GetMouseButtonDown(1))
        {
            if (isObjectSelected)
            {
                DeselectObject();
            }
        }
    }

    // 选中物体并高亮显示
    public static void SelectObject(GameObject obj)
    {
        if (isObjectSelected && selectedObj != obj)
        {
            Unhighlight();
        }

        selectedObj = obj;
        isObjectSelected = true;

        Highlight();
    }

    // 取消选中物体
    public static void DeselectObject()
    {
        if (isObjectSelected)
        {
            Unhighlight();
            isObjectSelected = false;
            selectedObj = null;
        }
    }

    // 获得选中的物体
    public static GameObject getSelectedObject()
    {
        return selectedObj;
    }

    // 高亮选中的物体
    private static void Highlight()
    {
        if (selectedObj != null)
        {
            Outline outline = selectedObj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = true;
            }
        }
    }

    // 取消高亮显示
    private static void Unhighlight()
    {
        if (selectedObj != null)
        {
            Outline outline = selectedObj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
    }

    // 执行射线检测
    public static GameObject RayToGetObj()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从鼠标位置发射射线
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // 如果射线击中了物体
        {
            return hit.collider.gameObject; // 返回被点击的物体
        }

        return null; // 如果没有击中物体，返回 null
    }
}

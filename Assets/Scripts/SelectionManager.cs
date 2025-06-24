using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static GameObject selectedObj;  // 当前选中的物体
    public static bool isObjectSelected = false;  // 标记物体是否已选中

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
}

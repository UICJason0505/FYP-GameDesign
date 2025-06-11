using UnityEngine;

public class UnitController : MonoBehaviour
{
    public bool isSelected = false;

    public void Select()
    {
        isSelected = true;
        Debug.Log("Unit Selected: " + gameObject.name);
        // TODO: 播放选中动画
    }

    public void Deselect()
    {
        isSelected = false;
        // TODO: 清除选中状态显示
    }
}

using UnityEngine;
using UnityEngine.UI;

public class SubPanelUI : MonoBehaviour
{
    public Image[] slots = new Image[9]; // 9个格子对应Image组件
    private int selectedIndex = -1; // 当前选中的编号（0-8），-1表示无选中

    public Color normalColor = Color.gray; // 未选中颜色
    public Color activeColor = Color.white; // 选中颜色

    void Start()
    {
        SetAllToNormal();
        gameObject.SetActive(false); // 初始不显示
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        // 数字键 1 ~ 9
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ToggleSlot(i);
            }
        }

        // 鼠标点击时重置
        if (Input.GetMouseButtonDown(0))
        {
            ClearSelection();
        }
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        SetAllToNormal();
        selectedIndex = -1;
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    private void ToggleSlot(int index)
    {
        if (selectedIndex == index)
        {
            // 已选中 → 再次按下取消
            slots[index].color = normalColor;
            selectedIndex = -1;
        }
        else
        {
            // 切换选择
            SetAllToNormal();
            slots[index].color = activeColor;
            selectedIndex = index;
        }
    }

    private void SetAllToNormal()
    {
        foreach (Image img in slots)
        {
            img.color = normalColor;
        }
    }

    private void ClearSelection()
    {
        if (selectedIndex >= 0)
        {
            slots[selectedIndex].color = normalColor;
            selectedIndex = -1;
        }
    }
}

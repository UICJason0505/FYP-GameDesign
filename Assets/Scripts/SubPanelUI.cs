using UnityEngine;
using UnityEngine.UI;

public class SubPanelUI : MonoBehaviour
{
    public Image[] slots = new Image[6];
    private int selectedIndex = -1;

    public Color normalColor = Color.gray;
    public Color activeColor = Color.white;

    void Start()
    {
        SetAllToNormal();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        // 数字键
        for (int i = 0; i < slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ToggleSlot(i);
            }
        }

        // 点击鼠标清空
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

    public void TogglePanel()
    {
        if (gameObject.activeSelf)
            HidePanel();
        else
            ShowPanel();
    }

    private void ToggleSlot(int index)
    {
        if (selectedIndex == index)
        {
            slots[index].color = normalColor;
            selectedIndex = -1;
        }
        else
        {
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

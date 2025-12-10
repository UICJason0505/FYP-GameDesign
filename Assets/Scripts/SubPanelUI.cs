using UnityEngine;
using UnityEngine.UI;

public class SubPanelUI : MonoBehaviour
{
    public Image[] slots = new Image[6];
    private int selectedIndex = -1;

    public Color normalColor = Color.white;
    public Color activeColor = Color.white;

    void Start()
    {
        SetAllToNormal();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;



        // 点击鼠标清空
        if (Input.GetMouseButtonDown(0))
        {
            ClearSelection();
        }
    }
    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        SetAllToNormal();
        slots[index].color = activeColor;
        selectedIndex = index;
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

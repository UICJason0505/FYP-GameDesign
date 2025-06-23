using UnityEngine;
using TMPro;
using UnityEngine.UI; // 添加UI命名空间用于Image

public class UnitInfoPanelController : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI numberText;
    public Image zIndicator;
    public Image xIndicator;
    public Image cIndicator;
    public Image vIndicator;

    //public SubPanelController SubPanelUI;

    private Color normalColor = Color.grey; // 默认灰色
    private Color activeColor = Color.white; // 70%亮度灰色
    
    // 当前选中的技能键
    private KeyCode? selectedKey = null;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        // 仅当面板可见时处理快捷键
        if (!gameObject.activeSelf) return;

        // 检测ZXCV按键状态
        CheckKeyPress(KeyCode.Z, zIndicator);
        CheckKeyPress(KeyCode.X, xIndicator);
        CheckKeyPress(KeyCode.C, cIndicator);
        CheckKeyPress(KeyCode.V, vIndicator);

        //if (Input.GetKeyDown(KeyCode.C))//King的C键显示子面板
        //{
        //    if (currentUnit.unitName == "King")
        //    {
        //        subPanel.Show();
        //    }
        //}
    }

    private void CheckKeyPress(KeyCode key, Image indicator)
    {
        if (Input.GetKeyDown(key))
        {
            ResetAllIndicators();
            indicator.color = activeColor;
            selectedKey = key;
        }
    }

    private void ResetAllIndicators()
    {
        zIndicator.color = normalColor;
        xIndicator.color = normalColor;
        cIndicator.color = normalColor;
        vIndicator.color = normalColor;
        selectedKey = null;
    }
    public void ShowUnit(string unitName, int blood)
    {
        //需要挂载在所有棋子中
        nameText.text = unitName;
        numberText.text = blood.ToString();
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        ResetAllIndicators(); // 每次关闭时重置按钮颜色
        gameObject.SetActive(false);
    }
}

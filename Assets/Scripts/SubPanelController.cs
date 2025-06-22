using UnityEngine;
using UnityEngine.UI;

public class SubPanelController : MonoBehaviour
{
    public GameObject panelRoot; // 子面板容器，默认隐藏

    void Start()
    {
        panelRoot.SetActive(false); // 初始隐藏
    }

    public void Show()
    {
        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }

    public void OnOptionSelected(int index)
    {
        Debug.Log("你选择了选项：" + index);
        // TODO：根据 index 执行技能逻辑，比如生成对应的棋子
        Hide(); // 选择完自动隐藏
    }
}

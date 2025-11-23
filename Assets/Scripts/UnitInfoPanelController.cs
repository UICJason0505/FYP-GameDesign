using UnityEngine;
using TMPro;
using UnityEngine.UI; // 添加UI命名空间用于Image

public class UnitInfoPanelController : MonoBehaviour
{
    public static UnitInfoPanelController Instance { get; private set; }
    
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI numberText;
    public Image zIndicator;
    public Image xIndicator;
    public Image cIndicator;
    public Image vIndicator;

    public SubPanelUI subPanelUI;

    [Header("违规提示UI")]
    public TextMeshProUGUI invalidActionText;
    public float invalidMessageDuration = 2f;

    private Color normalColor = Color.grey; 
    private Color activeColor = Color.white; 
    
    private KeyCode? selectedKey = null;

    void Awake()
    {
        gameObject.SetActive(false);
        if (invalidActionText != null)
        {
            invalidActionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        // 右键退出模式
        if (Input.GetMouseButtonDown(1) && selectedKey != null)
        {
            ExitCurrentMode();
            // ✅ 右键时刷新AP UI
            FindObjectOfType<TurnManager>()?.UpdateTurnText();
            return;
        }

        // 检查模式按键
        CheckMode(KeyCode.Z, zIndicator);
        CheckMode(KeyCode.X, xIndicator);
        CheckMode(KeyCode.V, vIndicator);

        // 单独处理C
        if (Input.GetKeyDown(KeyCode.C))
        {
            HandleModeSwitch(KeyCode.C, cIndicator);

            if (selectedKey == KeyCode.C && nameText.text == "King" && subPanelUI != null)
            {
                subPanelUI.ShowPanel();
            }
        }
    }

    private void CheckMode(KeyCode key, Image indicator)
    {
        if (Input.GetKeyDown(key))
        {
            HandleModeSwitch(key, indicator);
        }
    }

    private void HandleModeSwitch(KeyCode key, Image indicator)
    {
        if (selectedKey == null)
        {
            // 进入模式
            ResetAllIndicators();
            indicator.color = activeColor;
            selectedKey = key;
        }
        else if (selectedKey == key)
        {
            // 再次点击 → 退出
            ExitCurrentMode();
        }
        else
        {
            // 不允许直接切换 → 提示违规
            ShowInvalidAction("Invaild Action");
        }
    }

    private void ExitCurrentMode()
    {
        ResetAllIndicators();
        selectedKey = null;
    }

    private void ResetAllIndicators()
    {
        zIndicator.color = normalColor;
        xIndicator.color = normalColor;
        cIndicator.color = normalColor;
        vIndicator.color = normalColor;
    }

    public void ShowUnit(string unitName, int blood)
    {
        // 检查是否为当前阵营
        if (SelectionManager.selectedObj != null)
        {
            King king = SelectionManager.selectedObj.GetComponent<King>();
            if (king != null)
            {
                TurnManager tm = FindObjectOfType<TurnManager>();
                if (tm != null)
                {
                    Player currentPlayer = tm.players[tm.turnCount];
                    Player kingPlayer = king.player;

                    if (kingPlayer != currentPlayer)
                    {
                        Debug.Log($"当前回合属于 {currentPlayer.playerName}，无法查看 {kingPlayer.playerName} 的单位信息！");
                        ShowInvalidAction("Not your turn!");
                        return;
                    }
                }
            }
        }
        //是当前阵营，显示UI
        nameText.text = unitName;
        numberText.text = blood.ToString();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        ExitCurrentMode();
        gameObject.SetActive(false);
    }

    // ✅ 显示违规提示
    public void ShowInvalidAction(string message)
    {
        if (invalidActionText == null) return;
        invalidActionText.text = message;
        invalidActionText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideInvalidAction));
        Invoke(nameof(HideInvalidAction), invalidMessageDuration);
    }

    private void HideInvalidAction()
    {
        if (invalidActionText != null)
            invalidActionText.gameObject.SetActive(false);
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public int turnCount = 0;
    public int currentTurn = 1;
    public TMP_Text turnText;
    public Button nextTurnButton;

    void Start()
    {
        UpdateTurnText();
        nextTurnButton.onClick.AddListener(AdvanceTurn);
    }

    void AdvanceTurn()
    {
        currentTurn++;
        UpdateTurnText();
    }

    void UpdateTurnText()
    {
        turnText.text = "Turn: " + currentTurn.ToString();
    }
}

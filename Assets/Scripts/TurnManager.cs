using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public List<Player> players = new();
    private int turnCount = 0;
    public int fullTurn = 1;
    public int playerNum = 2;
    public TMP_Text turnText;
    public Button nextTurnButton;
    private String[] names = {"Red", "Blue", "Green", "Yellow" };

    void Start()
    {
        if (players.Count == 0)
        {
            for (int i = 0; i < playerNum; i++)
            {
                players.Add(new Player(i, names[i]));
            }
        }
        players[0].canOperate = true;
        UpdateTurnText();
        nextTurnButton.onClick.AddListener(AdvanceTurn);
    }

    void AdvanceTurn()
    {
        players[turnCount].canOperate = false;
        turnCount++;
        if (turnCount >= playerNum)
        {
            turnCount = 0;
            fullTurn++;
        }
        players[turnCount].canOperate = true;
        UpdateTurnText();
    }

    void UpdateTurnText()
    {
        turnText.text = "Turn: " + fullTurn.ToString();
    }
}

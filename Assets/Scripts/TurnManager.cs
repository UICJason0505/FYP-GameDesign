using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public List<Player> players = new();
    public int turnCount = 0;
    public int fullTurn = 1;
    public int playerNum = 2;
    public TMP_Text turnText;
    public Button nextTurnButton;
    private String[] names = {"Red", "Blue", "Green", "Yellow" };
    public TMP_Text actionPointText;
    public TMP_Text currentPlayerText;
    public Transform redSpawn;
    public Transform blueSpawn;
    public Transform greenSpawn;
    public Transform yellowSpawn;


    void Awake()
    {
        playerNum = GameSettings.playerNum;
        
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
    public void Update()
    {
        if (players[turnCount].actionPoints != 5)
        {
            UpdateTurnText();
        }
    }

    void AdvanceTurn()
    {
        players[turnCount].canOperate = false;
        turnCount++;

        if (turnCount >= playerNum)
        {
            turnCount = 0;
            fullTurn++;

            // Reset action points for all players at the end of a full turn
            foreach (var player in players)
            {
                player.ResetActionPoints();
            }
        }

        players[turnCount].canOperate = true;
        UpdateTurnText();
    }

    public void UpdateTurnText()
    {
        Debug.Log("Updating turn text...");
        // ✅ 改名称显示为 Round 而不是 Turn
        turnText.text = "Round: " + fullTurn.ToString();

        var currentPlayer = players[turnCount];

        // ✅ 更新行动点数
        actionPointText.text = "AP: " + currentPlayer.actionPoints.ToString();

        // ✅ 显示当前玩家的名称
        currentPlayerText.text = "Player: " + currentPlayer.playerName;
    }

}

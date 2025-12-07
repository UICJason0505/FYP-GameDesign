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
    private String[] names = { "Red", "Blue", "Green", "Yellow" };
    public TMP_Text actionPointText;
    public TMP_Text currentPlayerText;
    public Transform redSpawn;
    public Transform blueSpawn;
    public Transform greenSpawn;
    public Transform yellowSpawn;

    [Header("Prefabs")]
    public GameObject kingPrefab;

    void Awake()
    {
        playerNum = Mathf.Clamp(GameSettings.playerNum, 2, 4);
        if (players.Count == 0)
        {
            for (int i = 0; i < playerNum; i++)
            {
                players.Add(new Player(i, names[i]));
            }
        }
        SpawnKings();
        players[0].canOperate = true;
        UpdateTurnText();
        nextTurnButton.onClick.AddListener(AdvanceTurn);
    }
    public void Update()
    {
        UpdateTurnText();
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
        // ✅ 改名称显示为 Round 而不是 Turn
        turnText.text = fullTurn.ToString();

        var currentPlayer = players[turnCount];

        // ✅ 更新行动点数
        actionPointText.text = currentPlayer.actionPoints.ToString();

        // ✅ 显示当前玩家的名称
        currentPlayerText.text = currentPlayer.playerName;
    }

    void SpawnKings()
    {
        for (int i = 0; i < playerNum; i++)
        {
            Transform spawnPoint = null;
            switch (i)
            {
                case 0: spawnPoint = redSpawn; break;
                case 1: spawnPoint = blueSpawn; break;
                case 2: spawnPoint = greenSpawn; break;
                case 3: spawnPoint = yellowSpawn; break;
            }

            if (spawnPoint == null)
            {
                Debug.LogError($"Spawn point for {names[i]} not assigned!");
                continue;
            }

            // 实例化 King
            GameObject kingObj = Instantiate(kingPrefab, spawnPoint.position, Quaternion.identity);

            // 设置名字
            kingObj.name = $"{names[i]}_King";
            // 绑定 Player
            kingObj.GetComponent<King>().player = players[i];
            kingObj.GetComponent<King>().number = 5;
            Debug.Log(kingObj.GetComponent<King>().player.playerName);
            //阵营颜色
            kingObj.GetComponent<Renderer>().material = spawnPoint.GetComponent<SpriteRenderer>().material;

        }
    }

}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static HexMath;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject gameOverUI;   // 失败时显示的面板，可留空
    public TMP_Text gameOverText;
    public HexTile[] tiles;
    public GameObject attacker = null;
    [SerializeField] public List<GameObject> prefabs = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        tiles = FindObjectsOfType<HexTile>();
    }
    void Update()
    {

    }
    public void GameOver(string reason = null)
    {
        Time.timeScale = 0;
        gameOverUI.SetActive(true);
        if (gameOverText != null) gameOverText.text = reason;
    }
}

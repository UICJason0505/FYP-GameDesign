using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public HexGridGenerator[] grids;
    public static GameManager Instance { get; private set; }
    public GameObject gameOverUI;   // 失败时显示的面板，可留空
    public TMP_Text gameOverText;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
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

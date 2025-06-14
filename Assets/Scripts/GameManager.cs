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
        // 按 1 键让第一个棋盘沉下
        if (Input.GetKeyDown(KeyCode.Alpha1))
            grids[0].Sink();

        // 按 2 键让第一个棋盘上浮
        if (Input.GetKeyDown(KeyCode.Alpha2))
            grids[0].Rise();

        // 按 3 键让第二个棋盘沉下（可类推）
        if (Input.GetKeyDown(KeyCode.Alpha3))
            grids[1].Sink();

        // 按 4 键让第二个棋盘浮起
        if (Input.GetKeyDown(KeyCode.Alpha4))
            grids[1].Rise();
    }
    public void GameOver(string reason = null)
    {
        Time.timeScale = 0;
        gameOverUI.SetActive(true);
        if (gameOverText != null) gameOverText.text = reason;
    }
}

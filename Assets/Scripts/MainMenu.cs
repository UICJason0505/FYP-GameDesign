using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown playerNumDropdown;

    // 在这里手动维护你所有可能的地图名称
    // 顺序就是轮换顺序
    private static readonly string[] MapList = new string[]
    {
        "Main","Map1","Map2","Map3"
    };

    private const string LastMapKey = "LastMapIndex";

    public void StartGame()
    {
        // 玩家数量
        int selectedIndex = playerNumDropdown.value;
        int playerNum = selectedIndex + 2;
        GameSettings.playerNum = playerNum;

        // 获取上次玩的地图序号，没有则用 -1（从 0 开始）
        int lastMap = PlayerPrefs.GetInt(LastMapKey, -1);

        // 自动进入下一个
        int nextMapIndex = (lastMap + 1) % MapList.Length;

        // 保存序号以备下次使用
        PlayerPrefs.SetInt(LastMapKey, nextMapIndex);
        PlayerPrefs.Save();

        // 加载目标地图
        SceneManager.LoadScene(MapList[nextMapIndex]);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

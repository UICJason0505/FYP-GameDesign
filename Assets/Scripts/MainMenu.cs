using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame 被调用");

#if UNITY_EDITOR
        // 如果是在 Unity 编辑器中运行
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 如果是打包后的游戏
        Application.Quit();
#endif
    }
}
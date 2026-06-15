using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneLoader : MonoBehaviour
{
    public enum SceneType
    {
        MainMenu,
        ARScene
    }

    public void LoadARScene()
    {
        SceneManager.LoadScene(SceneType.ARScene.ToString());
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
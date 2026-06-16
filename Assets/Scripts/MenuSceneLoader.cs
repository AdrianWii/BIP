using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneLoader : MonoBehaviour
{
    public enum SceneType
    {
        FaceTracking,
        ObjectScaling,
        PlaneScene,
        PlayerHealth
    }

    public void LoadFaceTracking()
    {
        LoadScene(SceneType.FaceTracking);
    }

    public void LoadObjectScaling()
    {
        LoadScene(SceneType.ObjectScaling);
    }

    public void LoadPlaneScene()
    {
        LoadScene(SceneType.PlaneScene);
    }

    public void LoadPlayerHealth()
    {
        LoadScene(SceneType.PlayerHealth);
    }

    private void LoadScene(SceneType sceneType)
    {
        SceneManager.LoadScene(sceneType.ToString());
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
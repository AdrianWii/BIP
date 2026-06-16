using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu";

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
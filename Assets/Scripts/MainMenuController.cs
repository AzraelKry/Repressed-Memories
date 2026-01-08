using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameStartScene = "TriggerWarning";

    public void StartGame()
    {
        SceneManager.LoadScene(gameStartScene);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit called"); 
    }
}

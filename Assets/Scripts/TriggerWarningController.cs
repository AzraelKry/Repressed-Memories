using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerWarningController : MonoBehaviour
{
    public string firstGameScene = "Room1";   // set in inspector
    public string mainMenuScene = "MainMenu"; // set in inspector

    public void ContinueGame()
    {
        SceneManager.LoadScene(firstGameScene);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}


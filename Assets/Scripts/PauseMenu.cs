using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu I;

    public GameObject menuUI;

    private bool isPaused = false;

    void Awake()
    {
        I = this;
        menuUI.SetActive(false);
    }

    void Update()
    {
        if (DialogueManager.I != null && DialogueManager.I.IsShowing) return;
        if (IntrusiveThoughtUI.I != null && IntrusiveThoughtUI.I.canvas.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        menuUI.SetActive(false);
        PlayerMovement.Instance.CanMove = true;
        isPaused = false;
    }

    void Pause()
    {
        menuUI.SetActive(true);
        PlayerMovement.Instance.CanMove = false;
        isPaused = true;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}


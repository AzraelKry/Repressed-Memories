using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager I;
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text text;

    public Color phase1Color = Color.white;
    public Color phase2Color = new Color(1f, 0.6f, 0.6f); // soft red/pink


    public bool IsShowing { get; private set; }

    void Awake()
    {
        I = this;
        Hide();
    }

    public void Show(string line)
    {
        if (StageManager.Instance != null && StageManager.Instance.IsStage2)
            text.color = phase2Color;
        else
            text.color = phase1Color;


        panel.SetActive(true);
        text.text = line;
        IsShowing = true;

        if (PlayerMovement.Instance != null)
            PlayerMovement.Instance.CanMove = false;
    }

    public void Hide()
    {
        panel.SetActive(false);
        text.text = "";
        IsShowing = false;

        if (PlayerMovement.Instance != null)
            PlayerMovement.Instance.CanMove = true;

        if (StageManager.Instance != null)
            StageManager.Instance.TrySwitchAfterDialogue();
    }
}


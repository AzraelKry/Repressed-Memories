using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PsychManager : MonoBehaviour
{
    [System.Serializable]
    public class PsychQuestion
    {
        [TextArea] public string question;
        public string answerA;
        public string answerB;
        public string answerC;

        // 0 = A, 1 = B, 2 = C
        public int correctAnswerIndex;
    }

    [Header("Intro Dialogue")]
    [TextArea]
    public string[] introLines;

    [Header("Questions")]
    public PsychQuestion[] questions;

    [Header("UI References")]
    public TMP_Text questionText;

    public TMP_Text buttonAText;
    public TMP_Text buttonBText;
    public TMP_Text buttonCText;

    public Button buttonA;
    public Button buttonB;
    public Button buttonC;

    private int index = 0;
    private int score = 0;

    private bool playingIntro = true;
    private int introIndex = 0;

    void Start()
    {
        buttonA.onClick.AddListener(() => OnAnswer(0));
        buttonB.onClick.AddListener(() => OnAnswer(1));
        buttonC.onClick.AddListener(() => OnAnswer(2));

        ShowIntroLine();
    }

    void ShowIntroLine()
    {
        if (introIndex < introLines.Length)
        {
            questionText.text = introLines[introIndex];

            buttonAText.text = "Continue";
            buttonBText.text = "";
            buttonCText.text = "";

            buttonA.gameObject.SetActive(true);
            buttonB.gameObject.SetActive(false);
            buttonC.gameObject.SetActive(false);

            return;
        }

        playingIntro = false;

        buttonA.gameObject.SetActive(true);
        buttonB.gameObject.SetActive(true);
        buttonC.gameObject.SetActive(true);

        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        if (index >= questions.Length)
        {
            EndScene();
            return;
        }

        PsychQuestion q = questions[index];

        questionText.text = q.question;

        buttonAText.text = q.answerA;
        buttonBText.text = q.answerB;
        buttonCText.text = q.answerC;
    }

    void OnAnswer(int chosenIndex)
    {
        if (playingIntro)
        {
            introIndex++;
            ShowIntroLine();
            return;
        }

        PsychQuestion q = questions[index];

        if (chosenIndex == q.correctAnswerIndex)
            score++;

        index++;
        DisplayQuestion();
    }

    void EndScene()
    {
        bool goodEnding = score >= Mathf.CeilToInt(questions.Length * 0.5f);

        if (goodEnding)
            UnityEngine.SceneManagement.SceneManager.LoadScene("GoodEnding");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("BadEnding");
    }
}


using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("Room Stages")]
    public GameObject roomStage1;
    public GameObject roomStage2;

    [Header("Intrusive Thought Settings")]
    public int intrusiveMin = 2;
    public int intrusiveMax = 4;

    [Header("Intrusive Thought Intensity")]
    public float baseShakeStrength = 0.5f;       
    public float shakeIncreasePerThought = 0.5f;  

    [HideInInspector] public float currentShakeStrength = 0f;


    [TextArea]
    [SerializeField] private string[] intrusiveThoughts;

    public bool IsStage2 => stage2Active;

    private List<string> remainingThoughts;

    private int intrusiveCount = 0;
    private int intrusiveRequired = 0;
    private bool pendingIntrusiveThought = false;

    [Header("Phase Requirements (auto)")]
    private int stage1TotalObjects = 0;
    private int stage2TotalObjects = 0;
    private int stage1Interactions = 0;
    private int stage2Interactions = 0;

    private bool stage2Active = false;
    private bool pendingMoodSwitch = false;
    private bool pendingRoomSwitch = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (roomStage2 != null)
            roomStage2.SetActive(false);

        // intrusive interval
        intrusiveRequired = Random.Range(intrusiveMin, intrusiveMax + 1);
        intrusiveCount = 0;

        // NEW — set initial shake strength
        currentShakeStrength = baseShakeStrength;

        // count objects (include inactive for stage2)
        stage1TotalObjects = roomStage1 != null
            ? roomStage1.GetComponentsInChildren<Interactable>(true).Length
            : 0;

        stage2TotalObjects = roomStage2 != null
            ? roomStage2.GetComponentsInChildren<Interactable>(true).Length
            : 0;


        // intrusive thoughts pool
        remainingThoughts = new List<string>(intrusiveThoughts ?? new string[0]);

        // initialize UI counter
        ObjectCounterUI.I.UpdateCounter(0, stage1TotalObjects, false);

        Debug.Log($"[StageManager] INIT: Stage1 has {stage1TotalObjects} objects, Stage2 has {stage2TotalObjects} objects.");
        Debug.Log($"[StageManager] First intrusive thought after {intrusiveRequired} unique interactions.");
    }

    // called every time a NEW unique object is interacted with
    public void RegisterInteraction()
    {
        intrusiveCount++;
        Debug.Log($"[StageManager] RegisterInteraction - intrusiveCount = {intrusiveCount}/{intrusiveRequired}, stage2Active = {stage2Active}");

        // Flag intrusive thought if interval hit
        TryFlagIntrusiveThought();

        if (!stage2Active)
        {
            stage1Interactions++;
            Debug.Log($"[StageManager] Stage1 interactions: {stage1Interactions}/{stage1TotalObjects}");

            ObjectCounterUI.I.UpdateCounter(stage1Interactions, stage1TotalObjects, false);

            if (stage1Interactions >= stage1TotalObjects)
            {
                // Force one last intrusive thought before mood shift, if any thoughts left
                if (!pendingIntrusiveThought && remainingThoughts.Count > 0)
                {
                    pendingIntrusiveThought = true;
                    Debug.Log("[StageManager] All Stage1 objects seen - scheduling intrusive thought BEFORE mood switch.");
                }

                pendingMoodSwitch = true;
                Debug.Log("[StageManager] Stage1 complete - pendingMoodSwitch = true.");
            }
        }
        else
        {
            stage2Interactions++;
            Debug.Log($"[StageManager] Stage2 interactions: {stage2Interactions}/{stage2TotalObjects}");

            ObjectCounterUI.I.UpdateCounter(stage2Interactions, stage2TotalObjects, true);

            if (stage2Interactions >= stage2TotalObjects)
            {
                if (!pendingIntrusiveThought && remainingThoughts.Count > 0)
                {
                    pendingIntrusiveThought = true;
                    Debug.Log("[StageManager] All Stage2 objects seen - scheduling intrusive thought BEFORE room switch.");
                }

                pendingRoomSwitch = true;
                Debug.Log("[StageManager] Stage2 complete - pendingRoomSwitch = true.");
            }
        }
    }

    private void TryFlagIntrusiveThought()
    {
        if (intrusiveCount >= intrusiveRequired && remainingThoughts.Count > 0)
        {
            intrusiveCount = 0;
            intrusiveRequired = Random.Range(intrusiveMin, intrusiveMax + 1);
            pendingIntrusiveThought = true;

            Debug.Log($"[StageManager] Flagged intrusive thought. Next one will be after {intrusiveRequired} unique interactions.");
        }
    }

    // called when player closes *object description* (from DialogueManager.Hide)
    public void TrySwitchAfterDialogue()
    {
        Debug.Log($"[StageManager] TrySwitchAfterDialogue - pendingIntrusive={pendingIntrusiveThought}, pendingMood={pendingMoodSwitch}, pendingRoom={pendingRoomSwitch}, stage2Active={stage2Active}");

        // PRIORITY 1 — intrusive thought
        if (pendingIntrusiveThought)
        {
            pendingIntrusiveThought = false;
            StartCoroutine(TriggerIntrusiveThought());
            return;
        }

        // PRIORITY 2 — phase shift 
        if (pendingMoodSwitch && !stage2Active)
        {
            StartCoroutine(SwitchToStage2());
            return;
        }

        // PRIORITY 3 — room shift
        if (pendingRoomSwitch && stage2Active)
        {
            StartCoroutine(SwitchToNextRoom());
            return;
        }

        Debug.Log("[StageManager] TrySwitchAfterDialogue - nothing pending.");
    }

    private IEnumerator TriggerIntrusiveThought()
    {
        Debug.Log("[StageManager] TriggerIntrusiveThought - starting.");

        if (FadeController.I != null)
            yield return FadeController.I.FadeOut(0.6f);

        currentShakeStrength += shakeIncreasePerThought;

        string line = GetRandomIntrusiveText();
        Debug.Log($"[StageManager] Intrusive text: \"{line}\"");

        if (IntrusiveThoughtUI.I != null)
            IntrusiveThoughtUI.I.Show(line);

        // wait until intrusive thought UI is closed
        while (IntrusiveThoughtUI.I != null && IntrusiveThoughtUI.I.canvas.activeSelf)
            yield return null;

        if (FadeController.I != null)
            yield return FadeController.I.FadeIn(0.6f);

        Debug.Log("[StageManager] TriggerIntrusiveThought - finished. Re-checking pending transitions.");
        // After thought finishes, re-check mood/room switch flags
        TrySwitchAfterDialogue();
    }

    private string GetRandomIntrusiveText()
    {
        if (remainingThoughts == null || remainingThoughts.Count == 0)
        {
            Debug.LogWarning("[StageManager] No remaining intrusive thoughts. Returning fallback text.");
            return "...";
        }

        int index = Random.Range(0, remainingThoughts.Count);
        string line = remainingThoughts[index];
        remainingThoughts.RemoveAt(index);  // prevent repeats

        return line;
    }

    private IEnumerator SwitchToStage2()
    {
        Debug.Log("[StageManager] SwitchToStage2 - starting.");
        pendingMoodSwitch = false;

        if (FadeController.I != null)
            yield return FadeController.I.FadeOut(1f);

        stage2Active = true;

        currentShakeStrength = baseShakeStrength;

        // reset intrusive counter for the new phase
        intrusiveCount = 0;
        intrusiveRequired = Random.Range(intrusiveMin, intrusiveMax + 1);
        Debug.Log($"[StageManager] Stage2 active. Next intrusive after {intrusiveRequired} unique interactions.");

        if (roomStage1 != null) roomStage1.SetActive(false);
        if (roomStage2 != null) roomStage2.SetActive(true);

        ObjectCounterUI.I.UpdateCounter(0, stage2TotalObjects, true);

        if (FadeController.I != null)
            yield return FadeController.I.FadeIn(1f);

        Debug.Log("[StageManager] SwitchToStage2 - done.");
    }

    private IEnumerator SwitchToNextRoom()
    {
        Debug.Log("[StageManager] SwitchToNextRoom - starting.");
        pendingRoomSwitch = false;

        if (FadeController.I != null)
            yield return FadeController.I.FadeOut(1.2f);

        currentShakeStrength = baseShakeStrength;

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"[StageManager] Loading scene index {nextIndex}.");
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("[StageManager] No more rooms to load.");
        }
    }
}

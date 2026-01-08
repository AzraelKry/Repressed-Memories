using UnityEngine;
using TMPro;

public class ObjectCounterUI : MonoBehaviour
{
    public static ObjectCounterUI I;

    TMP_Text t;

    void Awake()
    {
        I = this;
        t = GetComponent<TMP_Text>();
    }

    public void UpdateCounter(int current, int total, bool stage2)
    {
        string label = stage2 ? "Revisited" : "Interacted";
        t.text = $"{label}: {current}/{total}";

        // Change color in stage 2
        if (stage2)
            t.color = new Color(1f, 0.3f, 0.35f); // soft red
        else
            t.color = Color.white;
    }
}

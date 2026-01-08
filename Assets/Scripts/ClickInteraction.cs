using UnityEngine;

public class ClickInteractor : MonoBehaviour
{
    public Camera cam;
    public LayerMask interactableMask;
    public Transform player;
    public GameObject prompt;

    private Interactable current;
    private Interactable lastHighlighted;

    void Update()
    {
        if (DialogueManager.I != null && DialogueManager.I.IsShowing)
        {
            prompt.SetActive(false);

            if (Input.GetKeyDown(KeyCode.F))
                DialogueManager.I.Hide();

            return;
        }

        if (IntrusiveThoughtUI.I != null && IntrusiveThoughtUI.I.canvas.activeSelf)
        {
            prompt.SetActive(false);

            if (Input.GetKeyDown(KeyCode.F))
                IntrusiveThoughtUI.I.Hide();

            return;
        }

        if (lastHighlighted != null && lastHighlighted != current)
            lastHighlighted.HighlightOff();

        current = GetNearestUsable();

        if (current != null)
        {
            current.HighlightOn();
            lastHighlighted = current;
        }
        else
        {
            if (lastHighlighted != null)
            {
                lastHighlighted.HighlightOff();
                lastHighlighted = null;
            }
        }

        prompt.SetActive(current != null);

        if (current != null && Input.GetKeyDown(KeyCode.F))
        {
            DialogueManager.I.Show(current.description);
            current.OnInteracted();
        }
    }


    private Interactable GetNearestUsable()
    {
        float maxDistance = 0.5f;

        Interactable nearest = null;
        float bestDist = Mathf.Infinity;

        Interactable[] all = FindObjectsOfType<Interactable>();

        Collider2D playerCol = player.GetComponent<Collider2D>();
        if (playerCol == null) return null;

        foreach (var it in all)
        {
            if (it.HasInteracted) continue;

            Collider2D hitCol = it.GetComponent<Collider2D>();
            if (hitCol == null) continue;

            float d = Physics2D.Distance(playerCol, hitCol).distance;

            if (d <= it.interactRange && d < bestDist && d <= maxDistance)
            {
                bestDist = d;
                nearest = it;
            }
        }

        return nearest;
    }
}


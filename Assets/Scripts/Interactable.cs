using UnityEngine;

public class Interactable : MonoBehaviour
{
    [TextArea] public string description;
    public float interactRange = 0.35f;
    public string baseId = "obj";

    private bool alreadyInteracted = false;
    public bool HasInteracted => alreadyInteracted;

    [Header("Highlight Colors")]
    public Color highlightStage1Color = new Color(1f, 0.8f, 0.9f, 1f); 
    public Color highlightStage2Color = new Color(0.6f, 0.6f, 0.6f, 1f); 

    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color; 
    }

    public bool CanInteract(Transform player)
    {
        Collider2D playerCol = player.GetComponent<Collider2D>();
        Collider2D myCol = GetComponent<Collider2D>();

        if (playerCol == null || myCol == null)
            return false;

        var dist = Physics2D.Distance(playerCol, myCol);

        return dist.distance <= interactRange;
    }

    public void OnInteracted()
    {
        if (!alreadyInteracted)
        {
            alreadyInteracted = true;

            HighlightOff(); 

            if (StageManager.Instance != null)
                StageManager.Instance.RegisterInteraction();
        }
    }

    public void HighlightOn()
    {
        if (sr == null) return;

        
        if (StageManager.Instance != null && StageManager.Instance.IsStage2)
            sr.color = highlightStage2Color;
        else
            sr.color = highlightStage1Color;
    }

    public void HighlightOff()
    {
        if (sr == null) return;

        sr.color = originalColor; 
    }
}




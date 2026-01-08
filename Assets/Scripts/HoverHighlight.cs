using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    [SerializeField] private float highlightAmount = 1.2f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void OnMouseEnter()
    {
        if (sr == null) return;

        Color c = originalColor;
        c *= highlightAmount;
        c.a = originalColor.a; 
        sr.color = c;
    }

    void OnMouseExit()
    {
        if (sr == null) return;
        sr.color = originalColor;
    }
}

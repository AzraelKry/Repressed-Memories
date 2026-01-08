using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image background;
    public TMP_Text label;

    [Header("Colors")]
    public Color normalColor = new Color(1f, 1f, 1f, 0f);   // fully transparent
    public Color hoverColor = new Color(1f, 1f, 1f, 0.15f); // light tint

    public Color normalTextColor = Color.white;
    public Color hoverTextColor = new Color(1f, 0.95f, 1f); // soft brighten

    void Start()
    {
        if (background != null) background.color = normalColor;
        if (label != null) label.color = normalTextColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (background != null) background.color = hoverColor;
        if (label != null) label.color = hoverTextColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (background != null) background.color = normalColor;
        if (label != null) label.color = normalTextColor;
    }
}


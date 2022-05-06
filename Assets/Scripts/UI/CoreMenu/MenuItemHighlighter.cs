using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuItemHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image image = null;

    Color unhighlightedColor = new Color(1, 1, 1, 0);
    Color highlightedColor = new Color(1, 1, 1, 1);

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = unhighlightedColor;
    }

    public void ForceUnhighlight()
    {
        image.color = unhighlightedColor;
    }
}

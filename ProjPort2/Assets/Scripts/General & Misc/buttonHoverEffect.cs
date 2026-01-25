using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class buttonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public float sizeDifference = 1.15f;

    private Vector2 originalSize;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale = originalSize * sizeDifference;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = originalSize;
    }

    public void OnSelect(BaseEventData eventData)
    {
        rectTransform.localScale = originalSize * sizeDifference;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        rectTransform.localScale = originalSize;
    }
}

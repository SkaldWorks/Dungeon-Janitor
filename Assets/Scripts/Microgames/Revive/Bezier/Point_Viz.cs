using UnityEngine;
using UnityEngine.EventSystems;

public class Point_Viz : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private RectTransform rectTransform;
    private RectTransform parentRect;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (rectTransform.parent != null)
            parentRect = rectTransform.parent as RectTransform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentRect == null)
            return;

        Camera eventCamera = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = canvas.worldCamera;
        }

        Vector2 localPosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventCamera,
                out localPosition))
        {
            rectTransform.anchoredPosition = localPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
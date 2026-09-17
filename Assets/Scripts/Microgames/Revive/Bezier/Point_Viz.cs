using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Point_Viz : MonoBehaviour, IDragHandler
{
    public bool movable = true;
    public Color movableColor = Color.white;
    public Color fixedColor = Color.gray;

    RectTransform rect;
    RectTransform parent;
    Canvas canvas;
    Image image;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        parent = rect.parent as RectTransform;
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        UpdateColor();
    }

    public void SetMovable(bool value)
    {
        movable = value;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (image != null)
            image.color = movable ? movableColor : fixedColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!movable || parent == null)
            return;

        Camera cam = canvas != null &&
                     canvas.renderMode != RenderMode.ScreenSpaceOverlay
                     ? canvas.worldCamera
                     : null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent, eventData.position, cam, out Vector2 pos))
        {
            rect.anchoredPosition = pos;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(RectTransform))]
public class TripwireDragHandle : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public TripwireMinigame minigame;

    [Header("Hover Animation")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverSpeed = 10f;
    [SerializeField] private Color hoverColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private float colorSpeed = 10f;

    [Header("Idle Pulse")]
    [SerializeField] private float pulseAmount = 0.05f;
    [SerializeField] private float pulseSpeed = 2f;

    private Image image;
    private RectTransform rectTransform;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private Color originalColor;
    private Color targetColor;

    private bool hovering;
    private bool dragging;
    private bool hasDragged;

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        image.raycastTarget = true;

        originalScale = rectTransform.localScale;
        targetScale = originalScale;

        originalColor = image.color;
        targetColor = originalColor;
    }

    private void Update()
    {
        if (!hasDragged && !dragging && !hovering)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            targetScale = originalScale * pulse;
        }
        else if (hovering && !dragging)
        {
            targetScale = originalScale * hoverScale;
        }

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * hoverSpeed
        );

        image.color = Color.Lerp(
            image.color,
            targetColor,
            Time.deltaTime * colorSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        targetScale = originalScale;
        targetColor = originalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        hasDragged = true;
        dragging = true;

        if (minigame != null)
        {
            minigame.BeginDragging(
                eventData.position,
                eventData.pressEventCamera);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (minigame != null)
        {
            minigame.DragWire(
                eventData.position,
                eventData.pressEventCamera);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;

        targetScale = originalScale;
        targetColor = originalColor;

        if (minigame != null)
            minigame.EndDragging();
    }
}

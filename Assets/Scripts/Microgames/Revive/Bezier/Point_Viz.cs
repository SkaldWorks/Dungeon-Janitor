
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Point_Viz : MonoBehaviour, IDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public bool movable = true;
    public Color movableColor = Color.white;
    public Color fixedColor = Color.gray;

    [Header("Hover Animation")]
    [SerializeField] float hoverScale = 1.1f;
    [SerializeField] float hoverSpeed = 10f;
    [SerializeField] Color hoverColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] float colorSpeed = 10f;

    RectTransform rect;
    RectTransform parent;
    Canvas canvas;
    Image image;

    Vector3 originalScale;
    Vector3 targetScale;
    Color targetColor;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        parent = rect.parent as RectTransform;
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();

        originalScale = rect.localScale;
        targetScale = originalScale;

        UpdateColor();
    }

    void Update()
    {
        rect.localScale = Vector3.Lerp(
            rect.localScale,
            targetScale,
            Time.deltaTime * hoverSpeed
        );

        if (image != null)
        {
            image.color = Color.Lerp(
                image.color,
                targetColor,
                Time.deltaTime * colorSpeed
            );
        }
    }

    public void SetMovable(bool value)
    {
        movable = value;
        targetScale = originalScale;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (image != null)
        {
            targetColor = movable ? movableColor : fixedColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!movable)
            return;

        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        UpdateColor();
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

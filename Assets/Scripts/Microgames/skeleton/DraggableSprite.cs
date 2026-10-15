
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class DraggableSprite : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Target")]
    public RectTransform target;

    [Header("Rotation")]
    [Min(0f)] public float rotationStep = 90f;
    [Range(0f, 180f)] public float rotationTolerance = 5f;

    [Header("Placement")]
    [Min(0f)] public float snapDistance = 50f;

    [Header("Transparency")]
    [Range(0f, 1f)]
    public float alphaHitTestThreshold = 0.001f;

    public RectTransform Target => target;
    public bool IsPlaced { get; private set; }

    private SkeletonMinigame minigame;
    private RectTransform rectTransform;
    private Image image;

    private Vector2 startPosition;
    private Quaternion startRotation;
    private bool hasStartTransform;
    private bool dragging;
    private Vector3 pointerOffset;

    internal void Initialize(SkeletonMinigame owner)
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        minigame = owner;

        image.raycastTarget = true;
        image.alphaHitTestMinimumThreshold = 0f;

        Sprite sprite = image.sprite;

        if (alphaHitTestThreshold > 0f && sprite != null)
        {
            if (sprite.texture.isReadable)
                image.alphaHitTestMinimumThreshold = alphaHitTestThreshold;
            else
                Debug.LogWarning("Enable Read/Write on the sprite texture.", this);
        }

        if (!hasStartTransform)
        {
            startPosition = rectTransform.anchoredPosition;
            startRotation = rectTransform.localRotation;
            hasStartTransform = true;
        }

        IsPlaced = false;
        dragging = false;
    }

    internal void ResetPiece()
    {
        if (!hasStartTransform)
            return;

        rectTransform.anchoredPosition = startPosition;
        rectTransform.localRotation = startRotation;

        IsPlaced = false;
        dragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag())
            return;

        RectTransform parent = rectTransform.parent as RectTransform;

        if (parent == null ||
            !RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parent, eventData.position, eventData.pressEventCamera,
                out Vector3 pointerPosition))
            return;

        pointerOffset = rectTransform.position - pointerPosition;
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || !CanDrag())
            return;

        RectTransform parent = rectTransform.parent as RectTransform;

        if (parent != null &&
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parent, eventData.position, eventData.pressEventCamera,
                out Vector3 pointerPosition))
        {
            rectTransform.position = pointerPosition + pointerOffset;
        }
    }

    private void Update()
    {
        if (!dragging || !CanDrag())
            return;

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            rectTransform.Rotate(
                Vector3.forward, -Mathf.Sign(scroll) * rotationStep,
                Space.Self);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging)
            return;

        dragging = false;

        if (CanDrag() &&
            minigame.TryPlace(this, eventData.position, eventData.pressEventCamera) &&
            IsCloseEnough() && IsCorrectRotation())
        {
            rectTransform.position = target.position;
            rectTransform.rotation = target.rotation;
            IsPlaced = true;
            minigame.PiecePlaced();
        }
        else
        {
            //ResetPiece();
        }
    }

    private bool CanDrag()
    {
        return minigame != null && minigame.IsRunning && !IsPlaced;
    }

    private bool IsCloseEnough()
    {
        return target != null &&
               (rectTransform.position - target.position).sqrMagnitude
               <= snapDistance * snapDistance;
    }

    private bool IsCorrectRotation()
    {
        return target != null &&
               Mathf.Abs(Mathf.DeltaAngle(
                   rectTransform.eulerAngles.z,
                   target.eulerAngles.z)) <= rotationTolerance;
    }
}

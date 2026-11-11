using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class DraggableSprite : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
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

    [Header("Hover Animation")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverSpeed = 10f;
    [SerializeField] private Color hoverColor = new Color(0.85f, 0.85f, 0.85f, 1f);
    [SerializeField] private float colorSpeed = 10f;

    [Header("Placement Effect")]
    public AudioSource audioSource;
    public AudioClip placementSound;

    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 1f;

    [SerializeField] private float popScale = 1.08f;
    [SerializeField] private float popDuration = 0.1f;
    [SerializeField] private float settleDuration = 0.15f;

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

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Color originalColor;
    private Color targetColor;

    private bool placementAnimating;
    private static bool anyPieceDragging;

    internal void Initialize(SkeletonMinigame owner)
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        minigame = owner;

        originalScale = rectTransform.localScale;
        targetScale = originalScale;
        originalColor = image.color;
        targetColor = originalColor;

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

        targetScale = originalScale;
        targetColor = originalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanDrag() || dragging || anyPieceDragging)
            return;

        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (dragging)
            return;

        targetScale = originalScale;
        targetColor = originalColor;
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
        anyPieceDragging = true;
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
        if (!placementAnimating)
        {
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
        anyPieceDragging = false;

        targetScale = originalScale;
        targetColor = originalColor;

        if (CanDrag() &&
            minigame.TryPlace(this, eventData.position, eventData.pressEventCamera) &&
            IsCloseEnough() && IsCorrectRotation())
        {
            rectTransform.position = target.position;
            rectTransform.rotation = target.rotation;
            IsPlaced = true;

            if (audioSource != null && placementSound != null)
                audioSource.PlayOneShot(placementSound, soundVolume);

            StartCoroutine(PlacementEffect());

            minigame.PiecePlaced();
        }
        else
        {
            //ResetPiece();
        }
    }

    private IEnumerator PlacementEffect()
    {
        placementAnimating = true;

        float elapsed = 0f;

        // Pop
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / popDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.localScale = Vector3.Lerp(
                originalScale,
                originalScale * popScale,
                t
            );

            yield return null;
        }

        elapsed = 0f;

        // Settle
        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / settleDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.localScale = Vector3.Lerp(
                originalScale * popScale,
                originalScale,
                t
            );

            yield return null;
        }

        rectTransform.localScale = originalScale;
        image.color = originalColor;

        placementAnimating = false;
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
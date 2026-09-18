using UnityEngine;

public class TripwireMinigame : MonoBehaviour, IMinigame
{
    [Header("UI")]
    public GameObject minigamePanel;

    [Header("Player")]
    public PlayerController player;

    [Header("Tripwire")]
    public RectTransform leftAnchor;
    public RectTransform rightAnchor;
    public RectTransform draggedEnd;
    public TripwireWireGraphic wireGraphic;

    [Header("Colliders")]
    public BoxCollider2D draggedCollider;
    public BoxCollider2D targetCollider;

    [Header("Reset")]
    public Vector2 resetOffset;

    private StartGame currentSource;
    private bool isPlaying;
    private bool dragging;

    private void Start()
    {
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (draggedCollider == null && draggedEnd != null)
            draggedCollider = draggedEnd.GetComponent<BoxCollider2D>();

        if (targetCollider == null && rightAnchor != null)
            targetCollider = rightAnchor.GetComponent<BoxCollider2D>();

        SetupColliders();
    }

    private void SetupColliders()
    {
        if (draggedCollider != null && draggedEnd != null)
        {
            draggedCollider.size = draggedEnd.rect.size;
            draggedCollider.offset = draggedEnd.rect.center;
        }

        if (targetCollider != null && rightAnchor != null)
        {
            targetCollider.size = rightAnchor.rect.size;
            targetCollider.offset = rightAnchor.rect.center;
        }
    }

    public void StartMinigame(StartGame source)
    {
        if (isPlaying)
            return;

        currentSource = source;
        isPlaying = true;
        dragging = false;

        if (player != null)
            player.freeze = true;

        if (minigamePanel != null)
            minigamePanel.SetActive(true);

        SetupColliders();
        ResetWire();

        if (wireGraphic != null)
            wireGraphic.Hide();
    }

    public void BeginDragging()
    {
        if (!isPlaying)
            return;

        dragging = true;

        if (wireGraphic != null)
            wireGraphic.Show();

        UpdateWire();
    }

    public void DragWire(Vector2 mousePosition)
    {
        if (!isPlaying || !dragging || draggedEnd == null)
            return;

        RectTransform parent = draggedEnd.parent as RectTransform;

        if (parent == null)
            return;

        Canvas canvas = draggedEnd.GetComponentInParent<Canvas>();

        Camera cam = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            mousePosition,
            cam,
            out Vector2 localPosition))
        {
            draggedEnd.anchoredPosition = localPosition;
        }

        UpdateWire();
    }

    public void EndDragging()
    {
        if (!isPlaying || !dragging)
            return;

        dragging = false;

        Physics2D.SyncTransforms();

        if (draggedCollider != null &&
            targetCollider != null &&
            draggedCollider.Distance(targetCollider).isOverlapped)
        {
            CompleteMinigame();
        }
        else
        {
            ResetWire();

            if (wireGraphic != null)
                wireGraphic.Hide();
        }
    }

    private void ResetWire()
    {
        if (draggedEnd == null || leftAnchor == null)
            return;

        draggedEnd.position =
            leftAnchor.position + (Vector3)resetOffset;

        UpdateWire();
    }

    private void UpdateWire()
    {
        if (wireGraphic == null)
            return;

        if (leftAnchor == null || draggedEnd == null)
            return;

        wireGraphic.SetPoints(
            leftAnchor.position,
            draggedEnd.position
        );
    }

    private void CompleteMinigame()
    {
        isPlaying = false;
        dragging = false;

        if (currentSource != null)
            currentSource.CompleteRepair();

        if (wireGraphic != null)
            wireGraphic.Hide();

        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (player != null)
            player.freeze = false;

        currentSource = null;
    }

    public void CancelMinigame()
    {
        if (!isPlaying)
            return;

        isPlaying = false;
        dragging = false;

        ResetWire();

        if (wireGraphic != null)
            wireGraphic.Hide();

        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (player != null)
            player.freeze = false;

        currentSource = null;
    }
}
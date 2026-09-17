using UnityEngine;

public class TripwireMinigame : MonoBehaviour, IMinigame
{
    [Header("Canvas")]
    public GameObject minigamePanel;

    [Header("Player")]
    public PlayerController player;

    [Header("Wire")]
    public RectTransform leftAnchor;
    public RectTransform rightAnchor;
    public RectTransform draggedEnd;
    public TripwireWireGraphic wireGraphic;

    [Header("Settings")]
    public float successDistance = 40f;

    public Vector2 resetOffset;

    private StartGame currentSource;
    private bool isPlaying;
    private bool dragging;

    private void Start()
    {
        if (minigamePanel != null)
            minigamePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            CheckForSuccess();
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

        ResetWire();
    }

    public void BeginDragging()
    {
        if (isPlaying)
            dragging = true;
    }

    public void DragWire()
    {
        if (!isPlaying || !dragging || draggedEnd == null)
            return;

        draggedEnd.position = Input.mousePosition;

        if (wireGraphic != null)
        {
            wireGraphic.SetPoints(
                leftAnchor.position,
                draggedEnd.position
            );
        }
    }

    private void ResetWire()
    {
        if (draggedEnd == null || leftAnchor == null)
            return;

        draggedEnd.position =
            leftAnchor.position + (Vector3)resetOffset;

        if (wireGraphic != null)
        {
            wireGraphic.SetPoints(
                leftAnchor.position,
                draggedEnd.position
            );
        }
    }

    private void CheckForSuccess()
    {
        if (draggedEnd == null || rightAnchor == null)
            return;

        float distance = Vector2.Distance(
            draggedEnd.position,
            rightAnchor.position
        );

        if (distance <= successDistance)
            CompleteMinigame();
        else
            ResetWire();
    }

    private void CompleteMinigame()
    {
        isPlaying = false;
        dragging = false;

        if (currentSource != null)
            currentSource.CompleteRepair();

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

        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (player != null)
            player.freeze = false;

        currentSource = null;
    }
}
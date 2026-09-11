using UnityEngine;
using UnityEngine.EventSystems;

public class TripwireMinigame : MonoBehaviour
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

    private Tripwire currentTripwire;
    private bool isPlaying = false;
    private bool dragging = false;

    private void Start()
    {
        if (minigamePanel != null)
            minigamePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        // Mouse button released
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;

            CheckForSuccess();
        }
    }

    public void StartMinigame(Tripwire tripwire)
    {
        if (isPlaying)
            return;

        currentTripwire = tripwire;
        isPlaying = true;

        if (player != null)
            player.freeze = true;

        if (minigamePanel != null)
            minigamePanel.SetActive(true);

        ResetWire();
    }

    private void ResetWire()
    {
        if (draggedEnd == null || leftAnchor == null)
            return;

        draggedEnd.position = leftAnchor.position;

        if (wireGraphic != null)
        {
            wireGraphic.SetPoints(
                leftAnchor.position,
                draggedEnd.position
            );
        }
    }

    public void BeginDragging()
    {
        if (!isPlaying)
            return;

        dragging = true;
    }

    public void DragWire()
    {
        if (!isPlaying || !dragging)
            return;

        Vector3 mousePosition = Input.mousePosition;

        // Keep the dragged point at the same screen depth.
        mousePosition.z = 0f;

        draggedEnd.position = mousePosition;

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
        {
            CompleteMinigame();
        }
        else
        {
            // Failed placement.
            // Put the wire back at the starting position.
            ResetWire();
        }
    }

    private void CompleteMinigame()
    {
        if (!isPlaying)
            return;

        isPlaying = false;
        dragging = false;

        if (currentTripwire != null)
        {
            currentTripwire.CompleteRepair();
        }

        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (player != null)
            player.freeze = false;

        currentTripwire = null;
    }
}
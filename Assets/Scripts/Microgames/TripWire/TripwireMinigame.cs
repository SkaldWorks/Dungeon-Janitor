
using UnityEngine;
using UnityEngine.UI;

public class TripwireMinigame : MonoBehaviour, IMinigame
{
    [Header("UI")]
    public GameObject minigamePanel;

    [Header("Player")]
    public PlayerController player;

    [Header("Tripwire")]
    public RectTransform leftAnchor;
    public RectTransform[] guideLoops;
    public RectTransform rightAnchor;
    public RectTransform draggedEnd;
    public TripwireGraphic wireGraphic;

    [Header("Starting Position")]
    public Vector2 resetOffset = new Vector2(100f, -40f);

    [Header("Guide Colors")]
    public Color inactiveColor =
        new Color(0.55f, 0.55f, 0.55f, 1f);

    public Color activeColor =
        new Color(1f, 0.7f, 0.15f, 1f);

    public Color completedColor =
        new Color(0.3f, 1f, 0.4f, 1f);

    private StartGame currentSource;
    private Image[] guideImages;

    private int nextGuideIndex;
    private bool isPlaying;
    private bool isDragging;
    private bool wasPlayerFrozen;

    private Vector3 dragOffset;

    private void Awake()
    {
        CacheGuideImages();

        if (minigamePanel != null)
            minigamePanel.SetActive(false);
    }

    private void CacheGuideImages()
    {
        if (guideLoops == null)
        {
            guideImages = new Image[0];
            return;
        }

        guideImages = new Image[guideLoops.Length];

        for (int i = 0; i < guideLoops.Length; i++)
        {
            if (guideLoops[i] == null)
                continue;

            guideImages[i] = guideLoops[i].GetComponent<Image>();

            if (guideImages[i] != null)
                guideImages[i].raycastTarget = false;
        }
    }

    public void StartMinigame(StartGame source)
    {
        if (isPlaying || !ValidateSetup(source))
            return;

        currentSource = source;
        nextGuideIndex = 0;
        isDragging = false;
        isPlaying = true;

        if (player != null)
        {
            wasPlayerFrozen = player.freeze;
            player.freeze = true;
        }

        minigamePanel.SetActive(true);

        ResetWirePosition();
        UpdateGuideVisuals();

        wireGraphic.Show();
        UpdateWire();
    }

    private bool ValidateSetup(StartGame source)
    {
        if (source == null ||
            minigamePanel == null ||
            leftAnchor == null ||
            rightAnchor == null ||
            draggedEnd == null ||
            wireGraphic == null)
        {
            Debug.LogError(
                "TripwireMinigame is missing a required reference.",
                this);

            return false;
        }

        if (guideLoops == null || guideLoops.Length < 2)
        {
            Debug.LogError(
                "TripwireMinigame requires at least two guide loops.",
                this);

            return false;
        }

        if (guideImages == null ||
            guideImages.Length != guideLoops.Length)
        {
            CacheGuideImages();
        }

        for (int i = 0; i < guideLoops.Length; i++)
        {
            if (guideLoops[i] == null || guideImages[i] == null)
            {
                Debug.LogError(
                    "Each guide loop needs a RectTransform and an Image.",
                    this);

                return false;
            }
        }

        if (!wireGraphic.CanDrawPath(guideLoops.Length))
        {
            Debug.LogError(
                "Create one more wire segment than the number of guide loops.",
                this);

            return false;
        }

        return true;
    }

    public void BeginDragging(
        Vector2 screenPosition,
        Camera eventCamera)
    {
        if (!isPlaying)
            return;

        RectTransform parent = draggedEnd.parent as RectTransform;

        if (parent == null)
            return;

        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parent,
                screenPosition,
                eventCamera,
                out Vector3 pointerPosition))
        {
            return;
        }

        dragOffset = draggedEnd.position - pointerPosition;
        isDragging = true;
    }

    public void DragWire(
        Vector2 screenPosition,
        Camera eventCamera)
    {
        if (!isPlaying || !isDragging)
            return;

        RectTransform parent = draggedEnd.parent as RectTransform;

        if (parent == null)
            return;

        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parent,
                screenPosition,
                eventCamera,
                out Vector3 pointerPosition))
        {
            return;
        }

        draggedEnd.position = pointerPosition + dragOffset;

        Vector2 endScreenPosition =
            RectTransformUtility.WorldToScreenPoint(
                eventCamera,
                draggedEnd.position);

        CheckProgress(endScreenPosition, eventCamera);

        if (isPlaying)
            UpdateWire();
    }

    private void CheckProgress(
        Vector2 screenPosition,
        Camera eventCamera)
    {
        if (nextGuideIndex < guideLoops.Length &&
            RectTransformUtility.RectangleContainsScreenPoint(
                guideLoops[nextGuideIndex],
                screenPosition,
                eventCamera))
        {
            nextGuideIndex++;
            UpdateGuideVisuals();
        }

        if (nextGuideIndex == guideLoops.Length &&
            RectTransformUtility.RectangleContainsScreenPoint(
                rightAnchor,
                screenPosition,
                eventCamera))
        {
            CompleteMinigame();
        }
    }

    public void EndDragging()
    {
        if (!isPlaying)
            return;

        isDragging = false;
    }

    private void ResetWirePosition()
    {
        if (leftAnchor.parent != null)
        {
            Vector3 localPosition =
                leftAnchor.localPosition +
                new Vector3(resetOffset.x, resetOffset.y, 0f);

            draggedEnd.position =
                leftAnchor.parent.TransformPoint(localPosition);
        }
        else
        {
            draggedEnd.position =
                leftAnchor.position + (Vector3)resetOffset;
        }
    }

    private void UpdateWire()
    {
        wireGraphic.SetPath(
            leftAnchor.position,
            guideLoops,
            nextGuideIndex,
            draggedEnd.position);
    }

    private void UpdateGuideVisuals()
    {
        for (int i = 0; i < guideImages.Length; i++)
        {
            if (guideImages[i] == null)
                continue;

            if (i < nextGuideIndex)
                guideImages[i].color = completedColor;
            else if (i == nextGuideIndex)
                guideImages[i].color = activeColor;
            else
                guideImages[i].color = inactiveColor;
        }
    }

    private void CompleteMinigame()
    {
        StartGame source = currentSource;

        CloseMinigame();

        if (source != null)
            source.CompleteRepair();
    }

    public void CancelMinigame()
    {
        if (!isPlaying)
            return;

        CloseMinigame();
    }

    private void CloseMinigame()
    {
        isPlaying = false;
        isDragging = false;

        if (wireGraphic != null)
            wireGraphic.Hide();

        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (player != null)
            player.freeze = wasPlayerFrozen;

        currentSource = null;
    }

    private void OnDisable()
    {
        if (isPlaying)
            CloseMinigame();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TripwireDragHandle : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IEndDragHandler
{
    public TripwireMinigame minigame;

    private Image handleImage;

    private void Awake()
    {
        handleImage = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (minigame != null)
            minigame.BeginDragging();

        if (handleImage != null)
            handleImage.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (minigame != null)
            minigame.DragWire();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (handleImage != null)
            handleImage.enabled = true;
    }
}
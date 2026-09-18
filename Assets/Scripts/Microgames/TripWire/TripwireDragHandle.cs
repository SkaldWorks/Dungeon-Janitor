using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TripwireDragHandle : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    public TripwireMinigame minigame;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (minigame != null)
            minigame.BeginDragging();

        if (image != null)
            image.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (minigame != null)
            minigame.DragWire(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (image != null)
            image.enabled = true;

        if (minigame != null)
            minigame.EndDragging();
    }
}
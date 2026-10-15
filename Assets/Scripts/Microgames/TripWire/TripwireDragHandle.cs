
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TripwireDragHandle : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [SerializeField] private TripwireMinigame minigame;

    private void Awake()
    {
        // The loose end must remain clickable.
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
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
        if (minigame != null)
            minigame.EndDragging();
    }
}

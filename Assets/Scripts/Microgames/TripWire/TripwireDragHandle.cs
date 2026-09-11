using UnityEngine;
using UnityEngine.EventSystems;

public class TripwireDragHandle : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler
{
    [SerializeField] private TripwireMinigame minigame;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (minigame != null)
        {
            minigame.BeginDragging();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (minigame != null)
        {
            minigame.DragWire();
        }
    }
}
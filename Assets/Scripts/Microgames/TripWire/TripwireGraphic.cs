using UnityEngine;

public class TripwireWireGraphic : MonoBehaviour
{
    public RectTransform wireRect;

    public void Show()
    {
        if (wireRect != null)
            wireRect.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (wireRect != null)
            wireRect.gameObject.SetActive(false);
    }

    public void SetPoints(Vector3 start, Vector3 end)
    {
        if (wireRect == null)
            return;

        Vector3 direction = end - start;

        wireRect.position = (start + end) / 2f;

        wireRect.sizeDelta = new Vector2(
            direction.magnitude,
            wireRect.sizeDelta.y
        );

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        wireRect.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
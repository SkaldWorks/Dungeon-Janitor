using UnityEngine;
using UnityEngine.UI;

public class TripwireWireGraphic : MonoBehaviour
{
    [SerializeField] private RectTransform wireRect;

    public void SetPoints(Vector3 start, Vector3 end)
    {
        if (wireRect == null)
            return;

        Vector3 direction = end - start;

        float distance = direction.magnitude;

        wireRect.position = (start + end) / 2f;

        wireRect.sizeDelta = new Vector2(
            distance,
            wireRect.sizeDelta.y
        );

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        wireRect.rotation = Quaternion.Euler(
            0f,
            0f,
            angle
        );
    }
}
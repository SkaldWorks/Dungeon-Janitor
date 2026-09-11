using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragNDrop : MonoBehaviour
{
    public bool isDragging = false;
    private Vector3 mouseStartPosition;
    private Vector3 spriteStartPosition;
    private void OnMouseDown()
    {
        isDragging = true;
        mouseStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spriteStartPosition = transform.localPosition;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.localPosition = spriteStartPosition + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseStartPosition);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }
}

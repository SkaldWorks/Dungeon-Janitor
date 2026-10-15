using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public float riseHeight = 3f;
    public float riseSpeed = 2f;

    [Header("Requirements")]
    public bool canOpen;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen;

    private void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * riseHeight;
    }

    private void Update()
    {
        if (isOpen)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                openPosition,
                riseSpeed * Time.deltaTime
            );
        }
    }

    public void Interact()
    {
        if (!canOpen || isOpen)
            return;

        isOpen = true;

    }

    public void OnTouchingPlayer()
    {
    }

    public void OnNotTouchingPlayer()
    {
    }

    public void SetCanOpen()
    {
        canOpen = true;
    }
}
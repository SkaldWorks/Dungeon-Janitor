
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public Rigidbody rb;
    public SpriteRenderer sr;

    public bool freeze = false;

    private Vector2 moveInput;

    private bool MovementAllowed =>
        !freeze &&
        (
            GameStateManager.Instance == null ||
            GameStateManager.Instance.CanPlayerMove
        );

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError(
                "PlayerController needs a Rigidbody.", this);

            enabled = false;
            return;
        }

        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        RestoreSavedRoomPosition();
    }

    private void RestoreSavedRoomPosition()
    {
        if (SaveManager.Instance == null)
            return;

        if (!SaveManager.Instance.TryGetCurrentRoomId(
                out string savedRoomId))
        {
            return;
        }

        RoomTasks room = RoomTasks.FindById(savedRoomId);

        if (room == null)
        {
            Debug.LogWarning(
                $"Saved room '{savedRoomId}' was not found in this scene. " +
                "The player will use the scene's default position.",
                this
            );

            return;
        }

        rb.position = room.SpawnPosition;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (!MovementAllowed || Keyboard.current == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;

        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.sKey.isPressed) y -= 1f;
        if (Keyboard.current.wKey.isPressed) y += 1f;

        moveInput = Vector2.ClampMagnitude(
            new Vector2(x, y), 1f);

        if (sr != null && x != 0f)
            sr.flipX = x < 0f;
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        if (!MovementAllowed)
        {
            moveInput = Vector2.zero;

            // Stop horizontal movement while preserving vertical velocity.
            Vector3 stoppedVelocity = rb.linearVelocity;
            stoppedVelocity.x = 0f;
            stoppedVelocity.z = 0f;

            rb.linearVelocity = stoppedVelocity;
            return;
        }

        Vector3 velocity = rb.linearVelocity;

        velocity.x = moveInput.x * speed;
        velocity.z = moveInput.y * speed;

        rb.linearVelocity = velocity;
    }

    public void freezePlayer(bool value)
    {
        freeze = value;

        if (value)
            moveInput = Vector2.zero;
    }
}

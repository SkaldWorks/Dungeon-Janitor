
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody rb;
    public SpriteRenderer sr;

    public bool freeze = false;

    private Vector2 moveInput;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("PlayerController needs a Rigidbody.", this);
            enabled = false;
            return;
        }

        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        if (freeze || Keyboard.current == null)
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

        moveInput = Vector2.ClampMagnitude(new Vector2(x, y), 1f);

        if (sr != null && x != 0f)
            sr.flipX = x < 0f;
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.x = moveInput.x * speed;
        velocity.z = moveInput.y * speed;

        rb.linearVelocity = velocity;
    }

    public void freezePlayer(bool value)
    {
        freeze = value;
    }
}

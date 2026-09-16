using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float groundDist;

    public LayerMask ground;
    public Rigidbody rb;
    public SpriteRenderer sr;

    public bool freeze = false;

    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (freeze)
        {
            moveInput = Vector2.zero;
            return;
        }

        float x = 0;
        float y = 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                x = -1;

            if (Keyboard.current.dKey.isPressed)
                x = 1;

            if (Keyboard.current.sKey.isPressed)
                y = -1;

            if (Keyboard.current.wKey.isPressed)
                y = 1;
        }

        moveInput = new Vector2(x, y);

        // Sprite flipping doesn't need to happen in FixedUpdate
        if (x < 0)
            sr.flipX = true;
        else if (x > 0)
            sr.flipX = false;
    }

    void FixedUpdate()
    {
        if (freeze)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 moveDir = new Vector3(
            moveInput.x,
            0,
            moveInput.y
        );

        rb.linearVelocity = moveDir * speed;

        RaycastHit hit;

        Vector3 castPos = rb.position;
        castPos.y += 1;

        if (Physics.Raycast(
            castPos,
            Vector3.down,
            out hit,
            Mathf.Infinity,
            ground))
        {
            Vector3 movePos = rb.position;
            movePos.y = hit.point.y + groundDist;

            rb.MovePosition(movePos);
        }
    }
}
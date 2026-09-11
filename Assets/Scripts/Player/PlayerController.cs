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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!freeze)
        {
            RaycastHit hit;
            Vector3 castPos = transform.position;
            castPos.y += 1;
            if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, ground))
            {
                if (hit.collider != null)
                {
                    Vector3 movePos = transform.position;
                    movePos.y = hit.point.y + groundDist;
                    transform.position = movePos;
                }
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

            Vector3 moveDir = new Vector3(x, 0, y);
            rb.linearVelocity = moveDir * speed;

            if (x != 0 && x < 0)
            {
                sr.flipX = true;
            }
            else if (x != 0 && x > 0)
            {
                sr.flipX = false;
            }
        }
    }
}

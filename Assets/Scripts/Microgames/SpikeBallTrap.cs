using UnityEngine;

public class SpikeBallTrap : MonoBehaviour, IMinigame
{
    public RectTransform swingArm;
    public RectTransform spikeBall;

    [Header("Swing")]
    public float swingForce = 2f;
    public float gravity = 180f;
    public float damping = 0.995f;
    public float maxSpeed = 300f;

    [Header("Lock")]
    [Range(90f, 175f)]
    public float targetAngle = 170f;

    public float overshoot = 5f;
    public float riseTime = 0.2f;
    public float fallTime = 0.4f;
    public float lockTime = 1f;

    private enum State
    {
        Swinging,
        Rising,
        Falling,
        Locked
    }

    private State state;

    private StartGame source;

    private float angle;
    private float speed;
    private float timer;

    private float lockAngle;
    private float overshootAngle;
    private float startAngle;

    private bool grabbed;

    private CursorLockMode oldLockMode;
    private bool oldCursorVisible;

    public void StartMinigame(StartGame source)
    {
        if (swingArm == null || spikeBall == null)
        {
            Debug.LogError("SpikeBallTrap: Assign Swing Arm and Spike Ball.");
            return;
        }

        Release();

        this.source = source;

        // The top of the arm is the pivot.
        swingArm.pivot = new Vector2(0.5f, 1f);

        // Attach the ball to the end of the arm.
        spikeBall.SetParent(swingArm, false);
        spikeBall.anchorMin = spikeBall.anchorMax = new Vector2(0.5f, 1f);
        spikeBall.anchoredPosition = new Vector2(
            0f,
            -swingArm.rect.height
        );

        angle = 0f;
        speed = 0f;
        timer = 0f;

        lockAngle = 0f;
        overshootAngle = 0f;
        startAngle = 0f;

        grabbed = false;
        state = State.Swinging;

        SetAngle(0f);

        gameObject.SetActive(true);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Swinging:
                HandleInput();
                Swing();

                if (Mathf.Abs(angle) >= targetAngle)
                    StartRising();

                break;

            case State.Rising:
                UpdateRising();
                break;

            case State.Falling:
                UpdateFalling();
                break;

            case State.Locked:
                UpdateLocked();
                break;
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && IsOverBall())
            Grab();

        if (Input.GetMouseButton(0) && grabbed)
        {
            speed = Mathf.Clamp(
                speed + Input.GetAxisRaw("Mouse X") * swingForce,
                -maxSpeed,
                maxSpeed
            );
        }

        if (Input.GetMouseButtonUp(0))
            Release();
    }

    private void Swing()
    {
        speed -=
            Mathf.Sin(angle * Mathf.Deg2Rad) *
            gravity *
            Time.deltaTime;

        speed *= Mathf.Pow(
            damping,
            Time.deltaTime * 60f
        );

        speed = Mathf.Clamp(
            speed,
            -maxSpeed,
            maxSpeed
        );

        angle += speed * Time.deltaTime;

        SetAngle(angle);
    }

    private void StartRising()
    {
        float direction = Mathf.Sign(angle);

        if (direction == 0f)
            direction = 1f;

        lockAngle = direction * targetAngle;

        overshootAngle = direction * Mathf.Min(
            targetAngle + overshoot,
            179f
        );

        startAngle = angle;
        timer = 0f;
        speed = 0f;

        Release();

        state = State.Rising;
    }

    private void UpdateRising()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(
            timer / Mathf.Max(0.01f, riseTime)
        );

        t = Mathf.SmoothStep(0f, 1f, t);

        angle = Mathf.Lerp(
            startAngle,
            overshootAngle,
            t
        );

        SetAngle(angle);

        if (t >= 1f)
        {
            timer = 0f;
            startAngle = angle;
            state = State.Falling;
        }
    }

    private void UpdateFalling()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(
            timer / Mathf.Max(0.01f, fallTime)
        );

        t = Mathf.SmoothStep(0f, 1f, t);

        angle = Mathf.Lerp(
            startAngle,
            lockAngle,
            t
        );

        SetAngle(angle);

        if (t >= 1f)
        {
            angle = lockAngle;
            timer = lockTime;
            state = State.Locked;

            // PLAY CLICK SOUND HERE.
            // Example:
            // clickSound.Play();
        }
    }

    private void UpdateLocked()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
            Complete();
    }

    private void SetAngle(float value)
    {
        angle = Mathf.Clamp(value, -179f, 179f);

        swingArm.localRotation = Quaternion.Euler(
            0f,
            0f,
            angle
        );
    }

    private bool IsOverBall()
    {
        Canvas canvas = spikeBall.GetComponentInParent<Canvas>();

        Camera cam = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(
            spikeBall,
            Input.mousePosition,
            cam
        );
    }

    private void Grab()
    {
        grabbed = true;

        oldLockMode = Cursor.lockState;
        oldCursorVisible = Cursor.visible;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Release()
    {
        if (!grabbed)
            return;

        grabbed = false;

        Cursor.lockState = oldLockMode;
        Cursor.visible = oldCursorVisible;
    }

    private void Complete()
    {
        Release();

        if (source != null)
            source.CompleteRepair();

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Release();
    }
}
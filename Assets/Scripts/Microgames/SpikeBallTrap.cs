
using UnityEngine;

public class SpikeBallTrap : MonoBehaviour, IMinigame
{
    [Header("References")]
    public RectTransform swingArm;
    public RectTransform spikeBall;

    [Header("Swing")]
    public float swingAcceleration = 120f;
    public float gravity = 180f;
    [SerializeField, Range(0f, 1f)] private float damping = 0.997f;
    public float maxSpeed = 180f;
    public float inputDeadzone = 0.5f;

    [Header("Lock")]
    [SerializeField, Range(45f, 120f)] private float targetAngle = 90f;
    public float overshoot = 3f;
    public float riseTime = 0.2f;
    public float fallTime = 0.4f;
    public float lockTime = 1f;

    private enum State
    {
        Idle,
        Swinging,
        Rising,
        Falling,
        Locked
    }

    private State state = State.Idle;
    private StartGame source;

    private float angle;
    private float speed;
    private float timer;

    private float lockAngle;
    private float overshootAngle;
    private float startAngle;

    private bool grabbed;
    private Vector2 previousPointerPosition;

    // Remember the cursor's original visibility so we can restore it.
    private bool previousCursorVisibility;
    private bool cursorVisibilitySaved;

    public void StartMinigame(StartGame source)
    {
        Release();

        if (swingArm == null || spikeBall == null)
        {
            Debug.LogError(
                "SpikeBallTrap: Assign Swing Arm and Spike Ball.",
                this
            );

            state = State.Idle;
            return;
        }

        this.source = source;

        swingArm.pivot = new Vector2(0.5f, 1f);

        spikeBall.SetParent(swingArm, false);
        spikeBall.anchorMin = new Vector2(0.5f, 1f);
        spikeBall.anchorMax = new Vector2(0.5f, 1f);
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
        Vector2 pointerPosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0) && IsOverBall())
        {
            grabbed = true;
            previousPointerPosition = pointerPosition;

            // Save the original state once, when the ball is grabbed.
            previousCursorVisibility = Cursor.visible;
            cursorVisibilitySaved = true;

            Cursor.visible = false;
        }

        if (grabbed && Input.GetMouseButton(0))
        {
            float deltaX =
                pointerPosition.x - previousPointerPosition.x;

            if (Mathf.Abs(deltaX) >= inputDeadzone)
            {
                float horizontalBoost = Mathf.Lerp(
                    1f,
                    2f,
                    Mathf.Abs(Mathf.Sin(angle * Mathf.Deg2Rad))
                );

                speed += Mathf.Sign(deltaX)
                       * swingAcceleration
                       * horizontalBoost
                       * Time.deltaTime;
            }

            previousPointerPosition = pointerPosition;
        }

        if (Input.GetMouseButtonUp(0))
            Release();
    }

    private void Swing()
    {
        float deltaTime = Time.deltaTime;

        speed -= Mathf.Sin(angle * Mathf.Deg2Rad)
               * gravity
               * deltaTime;

        speed *= Mathf.Pow(damping, deltaTime * 60f);
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        angle += speed * deltaTime;
        SetAngle(angle);
    }

    private void StartRising()
    {
        float direction = Mathf.Sign(angle);

        if (Mathf.Approximately(direction, 0f))
            direction = 1f;

        lockAngle = direction * targetAngle;

        float desiredOvershoot = Mathf.Min(
            targetAngle + overshoot,
            179f
        );

        overshootAngle = direction * Mathf.Max(
            Mathf.Abs(angle),
            desiredOvershoot
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

        angle = Mathf.Lerp(startAngle, overshootAngle, t);
        SetAngle(angle);

        if (t >= 1f)
        {
            angle = overshootAngle;
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

        angle = Mathf.Lerp(startAngle, lockAngle, t);
        SetAngle(angle);

        if (t >= 1f)
        {
            angle = lockAngle;
            timer = lockTime;
            state = State.Locked;
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
        Camera camera = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            camera = canvas.worldCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(
            spikeBall,
            Input.mousePosition,
            camera
        );
    }

    private void Release()
    {
        grabbed = false;

        if (!cursorVisibilitySaved)
            return;

        Cursor.visible = previousCursorVisibility;
        cursorVisibilitySaved = false;
    }

    private void Complete()
    {
        Release();

        if (source != null)
            source.CompleteRepair();

        gameObject.SetActive(false);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            Release();
    }

    private void OnDisable()
    {
        Release();
        state = State.Idle;
    }
}

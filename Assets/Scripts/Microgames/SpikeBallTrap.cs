using UnityEngine;

public class SpikeBallTrap : MonoBehaviour, IMinigame
{
    [Header("References")]
    [SerializeField] private RectTransform swingArm;
    [SerializeField] private RectTransform spikeBall;

    [Header("Physics")]
    [SerializeField, Min(0f)] private float gravity = 180f;
    [SerializeField, Min(0f)] private float swingAcceleration = 90f;
    [SerializeField, Min(0f)] private float damping = 0.8f;
    [SerializeField, Min(0f)] private float maxSpeed = 100f;

    [Header("Input")]
    [SerializeField, Min(0f)] private float inputStrength = 1f;
    [SerializeField, Min(0f)] private float pointerSensitivity = 0.005f;

    [Header("Goal")]
    [SerializeField, Range(45f, 120f)] private float targetAngle = 90f;
    [SerializeField, Min(0f)] private float overshoot = 3f;
    [SerializeField, Min(0.01f)] private float riseTime = 0.2f;
    [SerializeField, Min(0.01f)] private float fallTime = 0.4f;
    [SerializeField, Min(0f)] private float lockTime = 1f;

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

    private float startAngle;
    private float targetLockAngle;
    private float overshootAngle;

    private bool grabbed;
    private Vector2 previousPointerPosition;

    public void StartMinigame(StartGame source)
    {
        Release();

        if (swingArm == null || spikeBall == null)
        {
            Debug.LogError(
                "SpikeBallTrap: Missing UI references.",
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

        SetAngle(0f);

        state = State.Swinging;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        switch (state)
        {
            case State.Swinging:
                HandleInput();
                UpdatePhysics(deltaTime);

                if (Mathf.Abs(angle) >= targetAngle)
                    StartRising();

                break;

            case State.Rising:
                UpdateTransition(deltaTime, overshootAngle, riseTime);
                break;

            case State.Falling:
                UpdateTransition(deltaTime, targetLockAngle, fallTime);
                break;

            case State.Locked:
                timer -= deltaTime;

                if (timer <= 0f)
                    Complete();

                break;
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && IsOverBall())
        {
            grabbed = true;
            previousPointerPosition = Input.mousePosition;
        }

        if (grabbed && Input.GetMouseButton(0))
        {
            Vector2 currentPosition = Input.mousePosition;

            float deltaX = currentPosition.x -
                           previousPointerPosition.x;

            inputDirection = Mathf.Clamp(
                deltaX * pointerSensitivity,
                -1f,
                1f
            );

            previousPointerPosition = currentPosition;
        }
        else
        {
            inputDirection = 0f;
        }

        if (Input.GetMouseButtonUp(0))
            Release();
    }

    private float inputDirection;

    private void UpdatePhysics(float deltaTime)
    {
        float input = inputDirection * inputStrength;

        float gravityForce =
            -Mathf.Sin(angle * Mathf.Deg2Rad) * gravity;

        speed += (gravityForce + input * swingAcceleration)
                 * deltaTime;

        speed *= Mathf.Exp(-damping * deltaTime);

        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        angle += speed * deltaTime;

        SetAngle(angle);
    }

    private void StartRising()
    {
        float direction = Mathf.Sign(angle);

        if (Mathf.Approximately(direction, 0f))
            direction = 1f;

        targetLockAngle = direction * targetAngle;

        overshootAngle = direction * Mathf.Min(
            targetAngle + overshoot,
            179f
        );

        startAngle = angle;
        timer = 0f;

        Release();

        state = State.Rising;
    }

    private void UpdateTransition(
        float deltaTime,
        float destination,
        float duration)
    {
        timer += deltaTime;

        float t = Mathf.Clamp01(timer / duration);
        t = Mathf.SmoothStep(0f, 1f, t);

        angle = Mathf.Lerp(startAngle, destination, t);

        SetAngle(angle);

        if (timer >= duration)
        {
            angle = destination;
            SetAngle(angle);

            if (state == State.Rising)
            {
                startAngle = angle;
                timer = 0f;
                state = State.Falling;
            }
            else
            {
                timer = lockTime;
                state = State.Locked;
            }
        }
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

        Camera camera = canvas != null &&
                        canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;

        return RectTransformUtility.RectangleContainsScreenPoint(
            spikeBall,
            Input.mousePosition,
            camera
        );
    }

    private void Release()
    {
        grabbed = false;
        inputDirection = 0f;
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

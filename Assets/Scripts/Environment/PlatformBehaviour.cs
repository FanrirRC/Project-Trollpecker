using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    public enum PlatformType
    {
        TimedDisappear,
        TriggeredDisappear,
        Moving,
        ProximityMoving
    }

    public enum MoveAxis
    {
        X,
        Y,
        Z
    }

    public enum StartDirection
    {
        Positive,
        Negative
    }

    [Header("Platform Type")]
    public PlatformType platformType;

    // =========================
    // DISAPPEAR SETTINGS
    // =========================
    [Header("Timed Disappear")]
    public float disappearDelay = 1f;
    private float timer;
    private bool steppedOn;

    // =========================
    // MOVING PLATFORM
    // =========================
    [Header("Moving Platform")]
    public MoveAxis moveAxis = MoveAxis.X;
    public float moveDistance = 3f;
    public float backAndForthSpeed = 2f;
    public StartDirection startDirection = StartDirection.Positive;
    public bool smoothMovement = true;

    private Vector3 startPosition;
    private Vector3 positivePoint;
    private Vector3 negativePoint;

    private int moveDirectionSign = 1;

    private Vector3 lastPosition;
    public Vector3 PlatformDelta { get; private set; }

    // =========================
    // PROXIMITY MOVEMENT
    // =========================
    [Header("Proximity Movement")]
    public float activationRange = 5f;
    public float proximitySpeed = 4f;
    public Transform player;
    public bool proximityEnabled = true;

    private bool externallyTriggered = false;

    void Start()
    {
        startPosition = transform.position;

        Vector3 axis = GetAxisVector();

        positivePoint = startPosition + axis * moveDistance;
        negativePoint = startPosition - axis * moveDistance;

        lastPosition = transform.position;

        moveDirectionSign = (startDirection == StartDirection.Positive) ? 1 : -1;
    }

    void Update()
    {
        switch (platformType)
        {
            case PlatformType.TimedDisappear:
                HandleTimedDisappear();
                break;

            case PlatformType.TriggeredDisappear:
                if (externallyTriggered)
                    gameObject.SetActive(false);
                break;

            case PlatformType.Moving:
                HandleContinuousMovement();
                break;

            case PlatformType.ProximityMoving:
                HandleProximityMovement();
                break;
        }

        PlatformDelta = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    Vector3 GetAxisVector()
    {
        switch (moveAxis)
        {
            case MoveAxis.X: return Vector3.right;
            case MoveAxis.Y: return Vector3.up;
            case MoveAxis.Z: return Vector3.forward;
        }

        return Vector3.right;
    }

    // =========================================
    // TIMED DISAPPEAR
    // =========================================
    void HandleTimedDisappear()
    {
        if (steppedOn)
        {
            timer += Time.deltaTime;

            if (timer >= disappearDelay)
                gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (platformType != PlatformType.TimedDisappear)
            return;

        if (other.CompareTag("Player"))
        {
            steppedOn = true;

            if (disappearDelay <= 0f)
                gameObject.SetActive(false);
        }
    }

    // =========================================
    // CONTINUOUS PING-PONG MOVEMENT
    // =========================================
    void HandleContinuousMovement()
    {
        Vector3 axis = GetAxisVector();

        float speed = backAndForthSpeed;

        if (smoothMovement)
        {
            float distanceToEdge = moveDistance - Mathf.Abs(Vector3.Dot(transform.position - startPosition, axis));
            speed = Mathf.Lerp(0.5f, backAndForthSpeed, distanceToEdge / moveDistance);
        }

        transform.position += axis * moveDirectionSign * speed * Time.deltaTime;

        float offset = Vector3.Dot(transform.position - startPosition, axis);

        if (offset > moveDistance)
        {
            transform.position = startPosition + axis * moveDistance;
            moveDirectionSign = -1;
        }
        else if (offset < -moveDistance)
        {
            transform.position = startPosition - axis * moveDistance;
            moveDirectionSign = 1;
        }
    }

    // =========================================
    // PROXIMITY MOVEMENT
    // =========================================
    void HandleProximityMovement()
    {
        if (!proximityEnabled || player == null)
            return;

        float distance = Vector3.Distance(player.position, startPosition);

        if (distance <= activationRange)
        {
            Vector3 axis = GetAxisVector();
            transform.position += axis * proximitySpeed * Time.deltaTime;
        }
    }

    // =========================================
    // EXTERNAL SIGNALS
    // =========================================
    public void TriggerPlatform()
    {
        externallyTriggered = true;
    }

    public void ToggleProximity(bool state)
    {
        proximityEnabled = state;
    }

    public void PlayerSteppedOn()
    {
        if (platformType != PlatformType.TimedDisappear)
            return;

        if (!steppedOn)
        {
            steppedOn = true;

            if (disappearDelay <= 0f)
                gameObject.SetActive(false);
        }
    }
}
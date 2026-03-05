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

    [Header("Platform Type")]
    public PlatformType platformType;

    // =========================
    // DISAPPEAR SETTINGS
    // =========================
    [Header("Timed Disappear")]
    public float disappearDelay = 1f; // 0 = instant
    private float timer;
    private bool steppedOn;

    // =========================
    // BACK & FORTH MOVEMENT
    // =========================
    [Header("Back & Forth Movement")]
    public Vector3 moveDirection = Vector3.right;
    public float moveDistance = 3f;
    public float backAndForthSpeed = 2f;

    private Vector3 startPosition;
    private bool movingForward = true;

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
        lastPosition = transform.position;
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
                HandleBackAndForthMovement();
                break;

            case PlatformType.ProximityMoving:
                HandleProximityMovement();
                break;
        }

        PlatformDelta = transform.position - lastPosition;
        lastPosition = transform.position;
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

    // =========================
    // BACK & FORTH MOVEMENT
    // =========================
    void HandleBackAndForthMovement()
    {
        float step = backAndForthSpeed * Time.deltaTime;

        if (movingForward)
        {
            transform.position += moveDirection.normalized * step;

            if (Vector3.Distance(startPosition, transform.position) >= moveDistance)
                movingForward = false;
        }
        else
        {
            transform.position -= moveDirection.normalized * step;

            if (Vector3.Distance(startPosition, transform.position) <= 0.05f)
                movingForward = true;
        }
    }

    // =========================================
    // PROXIMITY MOVEMENT (ONE DIRECTION ONLY)
    // =========================================
    void HandleProximityMovement()
    {
        if (!proximityEnabled || player == null)
            return;

        float distance = Vector3.Distance(player.position, startPosition);

        if (distance <= activationRange)
        {
            transform.position += moveDirection.normalized * proximitySpeed * Time.deltaTime;
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
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float jumpForce = 8.75f;
    [SerializeField] private float fallMultiplier = 2f;
    [SerializeField] private float maxFallSpeed = -16.25f;

    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private Vector3 moveInput;
    private Vector3 velocity;

    private Vector3 defaultSpawnPosition;

    private PlatformBehaviour currentPlatform;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        defaultSpawnPosition = transform.position;

        if (Checkpoint.savedPosition != Vector3.zero)
        {
            transform.position = Checkpoint.savedPosition;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        bool isActuallyGrounded = controller.isGrounded || currentPlatform != null;

        if (context.performed && isActuallyGrounded)
        {
            velocity.y = jumpForce;
            currentPlatform = null; // detach from platform when jumping
        }

        if (context.canceled && velocity.y > 0)
        {
            velocity.y *= 0.5f;
        }
    }

    void Update()
    {
        // ----------------------------
        // Horizontal Movement
        // ----------------------------
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // ----------------------------
        // Gravity
        // ----------------------------
        bool isActuallyGrounded = controller.isGrounded || currentPlatform != null;

        if (isActuallyGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (velocity.y < 0)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        if (velocity.y < maxFallSpeed)
        {
            velocity.y = maxFallSpeed;
        }

        controller.Move(velocity * Time.deltaTime);

        // ----------------------------
        // Moving Platform Sync
        // ----------------------------
        if (!controller.isGrounded)
        {
            currentPlatform = null;
        }

        if (currentPlatform != null)
        {
            controller.Move(currentPlatform.PlatformDelta);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.TryGetComponent(out PlatformBehaviour platform))
        {
            currentPlatform = platform;
            platform.PlayerSteppedOn();
        }
    }

    public void ResetToCheckpoint()
    {
        if (Checkpoint.savedPosition != Vector3.zero)
        {
            transform.position = Checkpoint.savedPosition;
        }
        else
        {
            transform.position = defaultSpawnPosition;
        }

        velocity = Vector3.zero;
        currentPlatform = null;
    }
}
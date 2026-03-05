using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControllerFS : MonoBehaviour
{
    public float mouseSensitivity = 0.25f;

    private float xRotation = 0f;
    private Vector2 CameraInput;

    public void Camera(InputAction.CallbackContext context)
    {
        CameraInput = context.ReadValue<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = CameraInput.x * mouseSensitivity;
        float mouseY = CameraInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}

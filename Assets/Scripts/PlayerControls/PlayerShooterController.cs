using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooterController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    [Header("Settings")]
    public float bulletSpeed = 40f;
    public float bulletLifetime = 5f;

    private Vector2 shootInput;

    public void Shoot(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Fire();
    }

    void Fire()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        Vector3 direction = (targetPoint - bulletSpawnPoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * bulletSpeed;

        Destroy(bullet, bulletLifetime);
    }
}
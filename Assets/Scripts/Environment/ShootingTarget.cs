using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    [Header("Assigned Platform")]
    [SerializeField] private PlatformBehaviour linkedPlatform;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (linkedPlatform != null)
            {
                linkedPlatform.TriggerPlatform();
            }

            Destroy(collision.gameObject);
        }
    }
}
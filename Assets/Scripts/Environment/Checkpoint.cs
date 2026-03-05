using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Vector3 savedPosition = Vector3.zero;
    public static bool hasCheckpoint = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            savedPosition = collision.transform.position;
            hasCheckpoint = true;
        }
    }
}

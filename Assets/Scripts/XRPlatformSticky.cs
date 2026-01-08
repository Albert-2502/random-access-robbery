using UnityEngine;

public class XRPlatformSticky : MonoBehaviour
{
    private Vector3 lastPosition;
    private GameObject player;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = rb.position;
    }

    // Use FixedUpdate because the platform Rigidbody moves here
    void FixedUpdate()
    {
        if (player != null)
        {
            // Calculate the velocity of the platform this physics frame
            Vector3 platformMovement = rb.position - lastPosition;

            // Teleport the player's transform by the exact movement amount
            // This bypasses the Character Controller's friction/lag
            player.transform.position += platformMovement;
        }

        lastPosition = rb.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the root of what touched us is tagged Player
        if (other.transform.root.CompareTag("Player"))
        {
            player = other.transform.root.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player != null && other.transform.root.gameObject == player)
        {
            player = null;
        }
    }
}
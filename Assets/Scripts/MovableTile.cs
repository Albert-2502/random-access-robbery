using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    private int targetIndex = 0;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (waypoints.Length < 2) return;

        Vector3 targetPos = waypoints[targetIndex].position;
        Vector3 newPos = Vector3.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);

        if (Vector3.Distance(rb.position, targetPos) < 0.05f)
        {
            targetIndex++;
            if (targetIndex >= waypoints.Length) targetIndex = 0;
        }
    }
}
using UnityEngine;

public class Plat12Movement : MonoBehaviour
{
    public float speed = 8f;           // Speed of the enemy movement
    public float moveDistance = 40f;   // Distance to move forward and backward

    private Vector3 startPos;          // Initial position of the enemy
    private bool movingForward = true; // To track movement direction

    void Start()
    {
        // Record the starting position of the enemy
        startPos = transform.position;
    }

    void Update()
    {
        // Calculate the target positions for forward and backward movement on the Z-axis
        Vector3 forwardPos = startPos + Vector3.back * moveDistance;   // Move towards -Z
        Vector3 backwardPos = startPos;                                // Start position

        // Move the enemy forward or backward depending on the direction
        if (movingForward)
        {
            // Move forward along the Z-axis (towards -Z)
            transform.position = Vector3.MoveTowards(transform.position, forwardPos, speed * Time.deltaTime);

            // Check if the enemy has reached the forward target
            if (Vector3.Distance(transform.position, forwardPos) < 0.01f)
            {
                movingForward = false; // Switch to moving backward
            }
        }
        else
        {
            // Move backward along the Z-axis (back towards starting position)
            transform.position = Vector3.MoveTowards(transform.position, backwardPos, speed * Time.deltaTime);

            // Check if the enemy has reached the backward target (start position)
            if (Vector3.Distance(transform.position, backwardPos) < 0.01f)
            {
                movingForward = true; // Switch to moving forward
            }
        }
    }
}

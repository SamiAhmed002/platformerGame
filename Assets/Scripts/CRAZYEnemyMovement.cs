using UnityEngine;

public class CRAZYEnemyMovement : MonoBehaviour
{
    public float speed = 4f;            // Speed of the enemy movement
    public float moveDistance = 26f;    // Distance to move forward and backward
    public float chaseSpeed = 6f;       // Speed when chasing the player
    public float detectionRange = 5f;  // Range within which the enemy starts chasing the player
    public Transform player;            // Reference to the player

    private Vector3 startPos;           // Initial position of the enemy
    private bool movingForward = true;  // To track movement direction
    private bool isChasing = false;     // Whether the enemy is currently chasing the player
    public bool hasTeleported = false;  // Disables movement if enemy teleports

    void Start()
    {
        // Automatically find the player GameObject by its tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        // Ensure the player was found
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
          Debug.LogError("Player not found! Make sure the player GameObject is tagged as 'Player'.");
        }
        // Record the starting position of the enemy
        startPos = transform.position;
    }


    void Update()
    {
        if (!hasTeleported) {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the player is within the detection range
            if (distanceToPlayer <= detectionRange && SettingsManager.gameMode == 2)
            {
                isChasing = true;
            }
            else
            {
                isChasing = false;
            }

            if (isChasing)
            {
                // Chase the player
                ChasePlayer();
            }
            else
            {
                // Patrol between the two points
                Patrol();
            }
        }
    }

    void Patrol()
    {
        // Calculate the target positions for forward and backward movement on the X-axis
        Vector3 forwardPos = startPos + Vector3.right * moveDistance;
        Vector3 backwardPos = startPos;

        // Move the enemy forward or backward depending on the direction
        if (movingForward)
        {
            // Move forward along the X-axis
            transform.position = Vector3.MoveTowards(transform.position, forwardPos, speed * Time.deltaTime);

            // Check if the enemy has reached the forward target
            if (Vector3.Distance(transform.position, forwardPos) < 0.01f)
            {
                movingForward = false; // Switch to moving backward
            }
        }
        else
        {
            // Move backward along the X-axis
            transform.position = Vector3.MoveTowards(transform.position, backwardPos, speed * Time.deltaTime);

            // Check if the enemy has reached the backward target (start position)
            if (Vector3.Distance(transform.position, backwardPos) < 0.01f)
            {
                movingForward = true; // Switch to moving forward
            }
        }
    }

    void ChasePlayer()
    {
        // Move towards the player with chase speed
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }
}

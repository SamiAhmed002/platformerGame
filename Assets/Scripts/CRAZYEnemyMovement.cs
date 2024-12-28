using UnityEngine;

public class CRAZYEnemyMovement : MonoBehaviour
{
    public float speed = 4f;            // Speed of the enemy movement
    public float moveDistance = 26f;    // Distance to move forward and backward
    public float chaseSpeed = 6f;       // Speed when chasing the player
    public float detectionRange = 5f;   // Range within which the enemy starts chasing the player
    public Transform player;            // Reference to the player
    public Animator animator;           // Reference to the Animator component
    public GameObject linkedSurface;    // Optional surface to check for contact
    
    private Vector3 startPos;           // Initial position of the enemy
    private bool movingForward = true;  // To track movement direction
    private bool isChasing = false;     // Whether the enemy is currently chasing the player
    public bool hasTeleported = false;  // Disables movement if enemy teleports
    private Rigidbody rb;              // Reference to the Rigidbody component
    private bool isInContactWithSurface = false;

    void Start()
    {
        // Get references
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player GameObject is tagged as 'Player'.");
        }
        
        startPos = transform.position;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!hasTeleported) {
            // Handle gravity based on surface contact if a surface is linked
            if (linkedSurface != null && rb != null)
            {
                rb.useGravity = !isInContactWithSurface;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                isChasing = true;
            }
            else
            {
                isChasing = false;
            }

            if (isChasing)
            {
                animator.SetBool("isChasing", true);
                ChasePlayer();
            }
            else
            {
                animator.SetBool("isChasing", false);
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
                // Set rotation to show the enemy's back
                transform.rotation = Quaternion.Euler(0, 270, 0);
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
                // Reset rotation to face forward
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }

    void ChasePlayer()
    {
        // Move towards the player with chase speed
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (linkedSurface != null && collision.gameObject == linkedSurface)
        {
            isInContactWithSurface = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (linkedSurface != null && collision.gameObject == linkedSurface && !isChasing)
        {
            isInContactWithSurface = false;
            hasTeleported = true;
            rb.useGravity = true;
        }
    }
}

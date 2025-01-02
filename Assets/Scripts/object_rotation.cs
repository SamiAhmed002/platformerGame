using UnityEngine;

public class object_rotation : MonoBehaviour
{
    [Header("Tracking Settings")]
    [SerializeField] private bool trackPlayer = true;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool lockYAxis = true;
    [SerializeField] private Transform customTarget;

    private Transform target;

    private void Start()
    {
        // If we want to track the player, find them by tag
        if (trackPlayer)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        else
        {
            // Use custom target if player tracking is disabled
            target = customTarget;
        }
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        
        // Lock Y axis if specified
        if (lockYAxis)
        {
            direction.y = 0;
        }

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        // Smoothly rotate towards the target
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
    }
}

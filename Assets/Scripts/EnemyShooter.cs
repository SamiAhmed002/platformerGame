using UnityEngine;
using System.Collections.Generic;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootingInterval = 2f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private Transform shootingPoint;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    
    private Transform player;
    private float shootingTimer;

    private void Start()
    {
        // Find the player using tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shootingTimer = shootingInterval;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            // Look at player (only Y axis rotation)
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // Keep vertical rotation locked
            transform.rotation = Quaternion.LookRotation(direction);

            shootingTimer -= Time.deltaTime;

            if (shootingTimer <= 0)
            {
                Shoot();
                shootingTimer = shootingInterval;
            }
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || shootingPoint == null) return;

        // Calculate direction to player
        Vector3 direction = (player.position - shootingPoint.position).normalized;

        // Create projectile at shooting point position
        GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
        
        // Point the projectile in the direction it should travel
        projectile.transform.forward = direction;

        // Configure the ProjectileMoveScript
        if (projectile.TryGetComponent(out ProjectileMoveScript moveScript))
        {
            moveScript.speed = projectileSpeed;
            moveScript.rotate = false; // Disable rotation unless you want spinning projectiles
            
            // Make sure the projectile isn't colliding with the enemy
            if (projectile.TryGetComponent(out Collider projectileCollider))
            {
                Physics.IgnoreCollision(projectileCollider, GetComponent<Collider>());
            }

            // Ensure trails are properly set up
            if (moveScript.trails == null || moveScript.trails.Count == 0)
            {
                // Try to find TrailRenderer in children
                TrailRenderer[] trailRenderers = projectile.GetComponentsInChildren<TrailRenderer>();
                if (trailRenderers.Length > 0)
                {
                    moveScript.trails = new List<GameObject>();
                    foreach (var trail in trailRenderers)
                    {
                        moveScript.trails.Add(trail.gameObject);
                        trail.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                // Enable existing trails
                foreach (var trail in moveScript.trails)
                {
                    if (trail != null)
                    {
                        trail.SetActive(true);
                        
                        // Ensure trail renderer is enabled
                        if (trail.TryGetComponent<TrailRenderer>(out var trailRenderer))
                        {
                            trailRenderer.enabled = true;
                            trailRenderer.time = 0.5f; // Adjust trail length
                            trailRenderer.startWidth = 0.5f; // Adjust trail width
                            trailRenderer.endWidth = 0f; // Trail fades to nothing
                        }
                    }
                }
            }
        }
    }

    // Optional: Visualize the detection range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
} 
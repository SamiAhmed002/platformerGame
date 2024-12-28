using UnityEngine;

public class SimpleEnemyShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float projectileAccuracy = 100f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private bool rotateToTarget = true;

    private Transform target;
    private float nextFireTime;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (shootPoint == null)
            shootPoint = transform;
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        if (distanceToTarget <= detectionRange)
        {
            if (rotateToTarget)
            {
                // Look at target (only Y axis rotation)
                Vector3 direction = target.position - transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // Check if it's time to fire
            if (Time.time >= nextFireTime)
            {
                FireProjectile();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;

        // Spawn projectile at shoot point
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        // Get the ProjectileMoveScript
        if (projectile.TryGetComponent(out ProjectileMoveScript moveScript))
        {
            // Configure the projectile
            moveScript.speed = projectileSpeed;
            moveScript.accuracy = projectileAccuracy;

            // Point at target
            Vector3 direction = (target.position - shootPoint.position).normalized;
            projectile.transform.forward = direction;

            // Ignore collision with shooter
            if (projectile.TryGetComponent(out Collider projectileCollider))
            {
                if (TryGetComponent(out Collider shooterCollider))
                {
                    Physics.IgnoreCollision(projectileCollider, shooterCollider);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Visualize shoot point
        if (shootPoint != null && shootPoint != transform)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(shootPoint.position, 0.2f);
            Gizmos.DrawLine(transform.position, shootPoint.position);
        }
    }
} 
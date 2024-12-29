using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        // Destroy the projectile after lifetime seconds
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if hit player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Add damage dealing logic here
            // Example: collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }

        // Destroy the projectile on impact
        Destroy(gameObject);
    }
} 
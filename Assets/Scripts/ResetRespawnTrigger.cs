using UnityEngine;

public class ResetRespawnTrigger : MonoBehaviour
{
    private PlatformManager platformManager;

    void Start()
    {
        // Find the PlatformManager in the scene
        platformManager = FindFirstObjectByType<PlatformManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the colliding object is the player
        {
            // Reset the respawn count in PlatformManager
            platformManager.ResetRespawnCount();
            Debug.Log("Respawn count reset to 0 at the start of the solid/unsolid level.");
        }
    }
}

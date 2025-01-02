using UnityEngine;

public class FanPlatform : MonoBehaviour
{
    public float liftSpeed = 100f; // Speed of the upward movement

    private void OnTriggerStay(Collider other)
    {
        // Check if the object is the player
        if (other.CompareTag("Player"))
        {

            // Access the PlayerMovement script
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                // Continuously apply lift without max height
                playerMovement.ApplyVerticalLift(liftSpeed);

            }
        }
    }
}












using UnityEngine;

public class ResetGridRespawnTrigger : MonoBehaviour
{
    private Grid3DLayout grid3DLayout;

    void Start()
    {
        // Find the Grid3DLayout in the scene
        grid3DLayout = FindFirstObjectByType<Grid3DLayout>();

        if (grid3DLayout == null)
        {
            Debug.LogError("Grid3DLayout script not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the colliding object is the player
        {
            if (grid3DLayout != null)
            {
                // Reset the respawn count in Grid3DLayout
                grid3DLayout.respawnCount = 0;
                PlayerPrefs.SetInt("GridRespawnCount", 0); // Save the reset state
                PlayerPrefs.Save();

                Debug.Log("Grid respawn count reset to 0.");
            }
        }
    }
}

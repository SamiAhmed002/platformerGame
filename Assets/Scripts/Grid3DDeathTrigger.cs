using UnityEngine;
using UnityEngine.SceneManagement;

public class Grid3DDeathTrigger : MonoBehaviour
{
    public GameObject player; // Reference to the player object
    private Grid3DLayout grid3DLayout; // Reference to the Grid3DLayout script

    void Start()
    {
        grid3DLayout = FindFirstObjectByType<Grid3DLayout>();

        if (grid3DLayout == null)
        {
            Debug.LogError("Grid3DLayout script not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            // Notify Grid3DLayout about the death and respawn
            grid3DLayout.HandleRespawn();

            // Reload the scene
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);

            Debug.Log("Grid3D Layout Death Triggered. Scene Reloaded.");
        }
    }
}




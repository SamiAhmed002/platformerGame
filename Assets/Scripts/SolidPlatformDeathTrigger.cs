using UnityEngine;
using UnityEngine.SceneManagement;

public class SolidPlatformDeathTrigger : MonoBehaviour
{
    public GameObject player;
    private PlatformManager platformManager;

    void Start()
    {
        platformManager = FindFirstObjectByType<PlatformManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            // Notify PlatformManager about respawn
            platformManager.HandleRespawn();

            // Reload the scene
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);

            Debug.Log("Solid/Unsolid Level Death Triggered. Scene Reloaded.");
        }
    }
}

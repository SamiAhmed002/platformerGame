using UnityEngine;

public class PlayerTriggerScript : MonoBehaviour
{
    private GoodEndingCutSceneManager cutSceneManager;

    private void Start()
    {
        // Find the GoodEndingCutSceneManager in the scene
        cutSceneManager = FindObjectOfType<GoodEndingCutSceneManager>();

        if (cutSceneManager == null)
        {
            Debug.LogWarning("GoodEndingCutSceneManager not found in the scene!");
        }
    }

    // This function can be called when you want to trigger the planet animation
    public void TriggerPlanetAnimationFromPlayer()
    {
        if (cutSceneManager != null)
        {
            Debug.Log("Calling TriggerPlanetAnimation from PlayerTriggerScript.");
            cutSceneManager.TriggerPlanetAnimation();
        }
        else
        {
            Debug.LogWarning("CutsceneManager reference is missing!");
        }
    }
}

using UnityEngine;

public class PlanetAnimationTrigger : MonoBehaviour
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

    // This function will be called by the animation event at the last frame of the planet animation
    public void TriggerFadeOut()
    {
        if (cutSceneManager != null)
        {
            Debug.Log("Calling TriggerFadeOut from PlanetAnimationTrigger.");
            cutSceneManager.TriggerFadeOut(); // Call the fade-out animation in the panel
        }
        else
        {
            Debug.LogWarning("CutsceneManager reference is missing!");
        }
    }
}

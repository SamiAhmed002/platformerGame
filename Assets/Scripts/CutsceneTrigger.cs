using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public BadEndingCutsceneManager cutsceneManager; // Reference to the BadEndingCutsceneManager script

    // This function will be called by the animation event
    public void TriggerFadeOut()
    {
        if (cutsceneManager != null)
        {
            Debug.Log("Calling TriggerFadeOut from CutsceneTrigger.");
            cutsceneManager.TriggerFadeOut(); // Call the TriggerFadeOut function in BadEndingCutsceneManager
        }
        else
        {
            Debug.LogWarning("CutsceneManager reference is not assigned!");
        }
    }
}

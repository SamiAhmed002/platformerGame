using UnityEngine;

public class BadEndingCutsceneManager : MonoBehaviour
{
    public Animator panelAnimator; // Reference to the Panel Animator
    public string fadeOutTrigger = "PlayFadeOut"; // Name of the trigger in the Panel Animator

    // This function is called by the animation event
    public void TriggerFadeOut()
    {
        if (panelAnimator != null)
        {
            Debug.Log("Triggering fade-out animation...");
            panelAnimator.SetTrigger(fadeOutTrigger);
        }
        else
        {
            Debug.LogWarning("Panel Animator is not assigned!");
        }
    }
}

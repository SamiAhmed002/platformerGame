using UnityEngine;

public class GoodEndingCutSceneManager : MonoBehaviour
{
    public Animator goodEndingAnimator; // Animator for the good ending cutscene
    public Animator planetAnimator;     // Animator for the planet animation
    public Animator panelAnimator;      // Animator for the fade-out panel

    public string planetAnimationTrigger = "PlayPlanetAnimation"; // Trigger for planet animation
    public string fadeOutTrigger = "PlayFadeOut";                 // Trigger for fade-out animation

    // This function will be called by the animation event at the end of the good ending animation
    public void TriggerPlanetAnimation()
    {
        if (planetAnimator != null)
        {
            Debug.Log("Triggering planet animation...");
            planetAnimator.SetTrigger(planetAnimationTrigger); // Start planet animation
        }
        else
        {
            Debug.LogWarning("Planet Animator is not assigned!");
        }
    }

    // This function will be called by the animation event at the end of the planet animation
    public void TriggerFadeOut()
    {
        if (panelAnimator != null)
        {
            Debug.Log("Triggering fade-out animation...");
            panelAnimator.SetTrigger(fadeOutTrigger); // Start fade-out animation
        }
        else
        {
            Debug.LogWarning("Panel Animator is not assigned!");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPlatformManager : MonoBehaviour
{
    public List<GameObject> platforms;       // List of all platforms
    public List<GameObject> nonDisappearingPlatforms; // Platforms that won't disappear
    public float blinkInterval;      // Time between blinks
    public int blinkCount;    // Number of blinks before disappearing
    public float disappearDuration = 3f;   // How long a platform stays disappeared
    public Color blinkColor = Color.red;    // Color to blink (e.g., red)
    public Color defaultColor = new Color(0.54f, 0.48f, 0.48f, 1f); // Original platform color (897C7C in RGB)
    public float platformInterval = 1f;     // Time delay between platforms disappearing

    private bool sequenceStarted = false;

    void Start()
    {

        blinkInterval = 0.3f / (SettingsManager.gameMode + 1);      // Time between blinks
        blinkCount = 3 - SettingsManager.gameMode;
        // Start the disappearing sequence loop
        StartCoroutine(DisappearingSequenceLoop());
    }

    IEnumerator DisappearingSequenceLoop()
    {
        while (true) // Infinite loop for repeating the sequence
        {
            yield return StartCoroutine(StartDisappearingSequence());
        }
    }

    IEnumerator StartDisappearingSequence()
    {
        if (sequenceStarted) yield break;
        sequenceStarted = true;

        // Get a randomized list of platforms to disappear
        List<GameObject> randomizedPlatforms = GetRandomizedPlatforms();

        foreach (GameObject platform in randomizedPlatforms)
        {
            // Blink the platform before disappearing
            yield return StartCoroutine(BlinkPlatform(platform));

            // Notify the PortalGun to remove portals on the disappearing platform
            NotifyPortalGun(platform);

            // Make the platform disappear
            platform.SetActive(false);

            // Wait for the disappear duration
            yield return new WaitForSeconds(disappearDuration);

            // Reappear the platform
            platform.SetActive(true);

            // Reset platform color
            SetPlatformColor(platform, defaultColor);

            // Delay before the next platform starts its sequence
            yield return new WaitForSeconds(platformInterval);
        }

        sequenceStarted = false; // Reset the sequence flag
    }

    IEnumerator BlinkPlatform(GameObject platform)
    {
        Renderer renderer = platform.GetComponent<Renderer>();

        if (renderer == null) yield break;

        for (int i = 0; i < blinkCount; i++)
        {
            // Change to blink color (e.g., red)
            SetPlatformColor(platform, blinkColor);
            yield return new WaitForSeconds(blinkInterval);

            // Change back to default color
            SetPlatformColor(platform, defaultColor);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    void SetPlatformColor(GameObject platform, Color color)
    {
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    List<GameObject> GetRandomizedPlatforms()
    {
        // Create a new list excluding non-disappearing platforms
        List<GameObject> randomizedPlatforms = new List<GameObject>();

        foreach (GameObject platform in platforms)
        {
            if (!nonDisappearingPlatforms.Contains(platform))
            {
                randomizedPlatforms.Add(platform);
            }
        }

        // Shuffle the list to randomize the order
        for (int i = 0; i < randomizedPlatforms.Count; i++)
        {
            GameObject temp = randomizedPlatforms[i];
            int randomIndex = Random.Range(0, randomizedPlatforms.Count);
            randomizedPlatforms[i] = randomizedPlatforms[randomIndex];
            randomizedPlatforms[randomIndex] = temp;
        }

        return randomizedPlatforms;
    }

    void NotifyPortalGun(GameObject disappearingPlatform)
    {
        PortalGun portalGun = FindObjectOfType<PortalGun>();
        if (portalGun != null)
        {
            portalGun.RemovePortalsOnPlatform(disappearingPlatform);
        }
    }
}


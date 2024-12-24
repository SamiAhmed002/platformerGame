using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPlatformManagerForMovingPlatforms : MonoBehaviour
{
    public List<GameObject> platforms;       // List of all platforms
    public List<GameObject> nonDisappearingPlatforms; // Platforms that won't disappear
    public float blinkInterval = 0.3f;      // Time between blinks
    public int blinkCount = 3;              // Number of blinks before disappearing
    public float disappearDuration = 3f;   // How long a platform stays disappeared
    public Color blinkColor = Color.red;    // Color to blink
    public Color defaultColor = new Color(0.54f, 0.48f, 0.48f, 1f); // Original platform color
    public float platformInterval = 1f;     // Time delay between platforms disappearing

    private bool sequenceStarted = false;

    public float globalTimer = 0f; // Shared timer for all platforms
    public float cycleDuration = 0f; // Duration of a full movement cycle (all waypoints)

    void Update()
    {
        globalTimer += Time.deltaTime;
    }

    //void Start()
    //{
    //    // Automatically calculate the cycle duration based on the longest platform cycle
    //    cycleDuration = CalculateLongestCycleDuration();

    //    Debug.Log($"Calculated global cycle duration: {cycleDuration}");

    //    // Start the disappearing sequence loop
    //    StartCoroutine(DisappearingSequenceLoop());
    //}

    void Start()
    {
        // Automatically calculate the cycle duration based on the longest platform cycle
        cycleDuration = CalculateLongestCycleDuration();

        //Debug.Log($"Calculated global cycle duration: {cycleDuration}");

        // Start the disappearing sequence loop
        StartCoroutine(DisappearingSequenceLoop());
    }

    private float CalculateLongestCycleDuration()
    {
        float longestDuration = 0f;

        foreach (GameObject platform in platforms)
        {
            MovingPlatformForDisappearingPlatforms movingPlatform = platform.GetComponent<MovingPlatformForDisappearingPlatforms>();
            if (movingPlatform != null)
            {
                // Calculate cycle duration for this platform
                float pathLength = movingPlatform.GetTotalPathLength();
                float speed = movingPlatform.GetSpeed();
                float platformCycleDuration = pathLength / speed;

                if (platformCycleDuration > longestDuration)
                {
                    longestDuration = platformCycleDuration;
                }
            }
        }

        return longestDuration;
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
            MovingPlatformForDisappearingPlatforms movingPlatform = platform.GetComponent<MovingPlatformForDisappearingPlatforms>();

            // Blink the platform before disappearing
            yield return StartCoroutine(BlinkPlatform(platform));

            // Deactivate the platform
            if (movingPlatform != null)
            {
                movingPlatform.SetActiveState(false);
            }

            // Wait for the disappear duration
            yield return new WaitForSeconds(disappearDuration);

            // Reactivate the platform
            if (movingPlatform != null)
            {
                movingPlatform.SetActiveState(true);
            }

            // Reset platform color
            SetPlatformColor(platform, defaultColor);

            // Delay before the next platform starts its sequence
            yield return new WaitForSeconds(platformInterval);
        }

        sequenceStarted = false;
    }


    // Blink and disappear platforms
    IEnumerator BlinkPlatform(GameObject platform)
    {
        Renderer renderer = platform.GetComponent<Renderer>();

        if (renderer == null) yield break;

        for (int i = 0; i < blinkCount; i++)
        {
            // Change to blink color
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
}

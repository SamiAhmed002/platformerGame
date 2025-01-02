using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlatformRow
{
    public List<GameObject> platforms; // List of platforms in this row
}

public class ConcentricPlatformManager : MonoBehaviour
{
    public List<PlatformRow> platformRows = new List<PlatformRow>(); // List of rows, each containing platforms
    public int maxDisappearingPlatforms = 1; // Fewer platforms disappear at the same time
    public float blinkInterval = 0.5f;       // Time between blinks (slower for easier gameplay)
    public int blinkCount = 2;              // Fewer blinks to reduce reaction time needed
    public float disappearDuration = 2f;   // Shorter disappear time
    public Color blinkColor = Color.red;    // Color to blink
    public Color defaultColor = new Color(0.54f, 0.48f, 0.48f, 1f); // Default platform color
    public float rowDisappearInterval = 2f; // Longer delay between row resets
    public float platformDisappearInterval = 0.5f; // Delay between platforms disappearing in the same row

    private bool sequenceStarted = false;

    void Start()
    {
        // Start the row-by-row disappearing sequence
        StartCoroutine(RowDisappearingSequenceLoop());
    }

    IEnumerator RowDisappearingSequenceLoop()
    {
        while (true) // Infinite loop for repeating the sequence
        {
            yield return StartCoroutine(StartRowDisappearingSequence());
        }
    }

    IEnumerator StartRowDisappearingSequence()
    {
        if (sequenceStarted) yield break;
        sequenceStarted = true;

        foreach (PlatformRow row in platformRows)
        {
            // Randomize and limit the platforms to disappear in the current row
            List<GameObject> platformsToDisappear = GetRandomizedPlatforms(row.platforms);

            // Blink and disappear platforms in this row with a delay
            foreach (GameObject platform in platformsToDisappear)
            {
                StartCoroutine(HandlePlatformDisappearance(platform));
                yield return new WaitForSeconds(platformDisappearInterval); // Delay between platform disappearances
            }

            // Wait before starting the next row
            yield return new WaitForSeconds(rowDisappearInterval);
        }

        sequenceStarted = false; // Reset the sequence flag
    }

    IEnumerator HandlePlatformDisappearance(GameObject platform)
    {
        // Blink the platform
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
    }

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

    List<GameObject> GetRandomizedPlatforms(List<GameObject> row)
    {
        // Create a copy of the row to shuffle
        List<GameObject> randomizedPlatforms = new List<GameObject>(row);

        // Shuffle the list
        for (int i = 0; i < randomizedPlatforms.Count; i++)
        {
            GameObject temp = randomizedPlatforms[i];
            int randomIndex = Random.Range(0, randomizedPlatforms.Count);
            randomizedPlatforms[i] = randomizedPlatforms[randomIndex];
            randomizedPlatforms[randomIndex] = temp;
        }

        // Limit the number of platforms to disappear
        int count = Mathf.Min(maxDisappearingPlatforms, randomizedPlatforms.Count);
        return randomizedPlatforms.GetRange(0, count);
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


using UnityEngine;
using TMPro; // For TextMeshPro
using System.Collections;

public class SubtitleManager : MonoBehaviour
{
    public TMP_Text subtitleText; // Reference to your subtitle text object
    public GameObject subtitleBackground; // Reference to the black background panel

    public void DisplaySubtitlesWithAudio(string[] lines, float[] startTimes, AudioSource audioSource)
    {
        StartCoroutine(SyncSubtitles(lines, startTimes, audioSource));
    }

    private IEnumerator SyncSubtitles(string[] lines, float[] startTimes, AudioSource audioSource)
    {
        subtitleText.text = ""; // Clear any existing subtitles
        subtitleBackground.SetActive(false); // Ensure background is initially hidden

        for (int i = 0; i < lines.Length; i++)
        {
            // Wait until the audio reaches the start time for the current subtitle
            while (audioSource.time < startTimes[i])
            {
                yield return null; // Wait for the next frame
            }

            // Show the current subtitle and background
            //subtitleBackground.SetActive(true);
            subtitleText.text = lines[i];
            subtitleBackground.SetActive(true);

            // Wait until the next subtitle or the end of the clip
            if (i < startTimes.Length - 1)
            {
                yield return new WaitUntil(() => audioSource.time >= startTimes[i + 1]);
            }
            else
            {
                yield return new WaitUntil(() => !audioSource.isPlaying); // Wait until the audio stops
            }

            // Clear the current subtitle
            subtitleText.text = "";
            subtitleBackground.SetActive(false); // Hide the background when no subtitles
        }
    }
}


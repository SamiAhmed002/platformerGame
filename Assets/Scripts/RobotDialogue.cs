using System.Collections;
using UnityEngine;
using TMPro;

public class RobotDialogue : MonoBehaviour
{
    public string[] dialogueLines; // Array of dialogue lines for this robot
    public float typewriterSpeed = 0.05f; // Speed of the typewriter effect
    private bool isInteracting = false; // Prevent multiple interactions
    private Coroutine dialogueCoroutine; // Reference to the current dialogue coroutine

    public void StartInteraction(TextMeshProUGUI subtitlesText, GameObject subtitlesContainer)
    {
        if (isInteracting)
        {
            return; // Prevent overlapping interactions
        }

        if (subtitlesText == null || subtitlesContainer == null)
        {
            return;
        }

        dialogueCoroutine = StartCoroutine(PlayDialogue(subtitlesText, subtitlesContainer));
    }

    public void StopDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine); // Stop the current dialogue coroutine
        }

        isInteracting = false; // Reset interaction flag
    }

    private IEnumerator PlayDialogue(TextMeshProUGUI subtitlesText, GameObject subtitlesContainer)
    {
        isInteracting = true;
        subtitlesContainer.SetActive(true);

        foreach (string line in dialogueLines)
        {
            yield return StartCoroutine(TypeLine(line, subtitlesText));
            yield return new WaitForSeconds(1f); // Delay between lines
        }

        subtitlesContainer.SetActive(false); // Hide subtitles after dialogue
        isInteracting = false;
        FindObjectOfType<RobotInteraction>().EndInteraction();
    }

    private IEnumerator TypeLine(string line, TextMeshProUGUI subtitlesText)
    {
        subtitlesText.text = ""; // Clear text

        foreach (char letter in line.ToCharArray())
        {
            subtitlesText.text += letter; // Add each letter
            yield return new WaitForSeconds(typewriterSpeed); // Wait between letters
        }
    }
}

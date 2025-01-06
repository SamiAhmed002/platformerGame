using System.Collections;
using UnityEngine;
using TMPro;

public class RobotDialogue : MonoBehaviour
{
    public string[] dialogueLines; // Array of dialogue lines for this robot
    public float typewriterSpeed = 0.05f; // Speed of the typewriter effect
    private bool isInteracting = false; // Prevent multiple interactions
    private Coroutine dialogueCoroutine; // Reference to the current dialogue coroutine

    public delegate void DialogueFinishedHandler();
    public event DialogueFinishedHandler onDialogueFinished; // Event to signal when dialogue is finished

    public void StartInteraction(TextMeshProUGUI subtitlesText, GameObject subtitlesContainer)
    {
        if (isInteracting)
        {
            return; // Prevent overlapping interactions
        }

        if (subtitlesText == null || subtitlesContainer == null)
        {
            Debug.LogError("SubtitlesText or SubtitlesContainer is not assigned!");
            return;
        }

        Debug.Log("Starting interaction with robot."); // Add this line
        isInteracting = true;
        subtitlesContainer.SetActive(true);
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
        foreach (string line in dialogueLines)
        {
            subtitlesText.text = ""; // Clear text

            foreach (char letter in line.ToCharArray())
            {
                subtitlesText.text += letter; // Add each letter
                yield return new WaitForSeconds(typewriterSpeed); // Wait between letters
            }

            yield return new WaitForSeconds(1f); // Delay between lines
        }

        subtitlesContainer.SetActive(false); // Hide subtitles after dialogue
        isInteracting = false;

        Debug.Log("Dialogue finished. Invoking event."); // Add this line
        onDialogueFinished?.Invoke(); // Notify that the dialogue has finished
    }

}
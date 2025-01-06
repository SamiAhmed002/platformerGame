using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class TriggerRobotManager : MonoBehaviour
{
    public GameObject robot; // The robot to appear/disappear
    public string[] dialogueLines; // Dialogue for this robot
    public float typewriterSpeed = 0.05f; // Speed for typewriter effect

    [Header("Countdown Settings (Final Robot Only)")]
    public bool isFinalRobot = false; // Check if this is the final robot
    public TMP_Text countdownText; // UI Text for the countdown timer
    public int countdownTime = 30; // Countdown time in seconds
    public string goodEndingScene = "GoodEndingCutscene";
    public string badEndingScene = "BadEndingCutscene";

    [Header("Sound Settings")]
    public AudioClip countdownSound; // Sound to play when countdown starts
    private AudioSource audioSource; // AudioSource to play the sound

    private RobotDialogue robotDialogue; // Reference to the RobotDialogue script
    private bool isCountdownRunning = false;
    private bool hasGoodEndingTriggered = false;

    private void Start()
    {
        robot.SetActive(false); // Start with the robot hidden
        robotDialogue = robot.GetComponent<RobotDialogue>();

        // Assign dialogue and typewriter speed
        if (robotDialogue != null)
        {
            robotDialogue.dialogueLines = dialogueLines;
            robotDialogue.typewriterSpeed = typewriterSpeed;
            robotDialogue.onDialogueFinished += OnDialogueFinished;
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false); // Hide countdown text initially
        }

        // Get or Add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            // Set default AudioSource properties
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // Make it fully 3D sound
            audioSource.volume = 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            robot.SetActive(true); // Show the robot
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            robot.SetActive(false); // Hide the robot

            // Stop the dialogue if it's ongoing
            if (robotDialogue != null)
            {
                robotDialogue.StopDialogue();
            }

            // Hide the subtitles container
            var player = other.GetComponent<RobotInteraction>();
            if (player != null)
            {
                player.subtitlesContainer.SetActive(false);
                player.EndInteraction(); // Reset the interaction flag on the player
            }
        }
    }

    private void OnDialogueFinished()
    {
        if (isFinalRobot)
        {
            Debug.Log("Final robot dialogue finished. Starting countdown.");
            StartCountdown();
        }
    }

    private void StartCountdown()
    {
        if (!isCountdownRunning)
        {
            Debug.Log("Starting countdown...");
            if (countdownSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(countdownSound); // Play sound when countdown starts
            }
            StartCoroutine(CountdownCoroutine());
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        isCountdownRunning = true;
        int currentTime = countdownTime;

        countdownText.gameObject.SetActive(true); // Show countdown text

        while (currentTime > 0)
        {
            countdownText.text = $"00:{currentTime:D2}";
            yield return new WaitForSeconds(1f);
            currentTime--;

            if (hasGoodEndingTriggered)
            {
                yield break; // Stop the countdown if the good ending is triggered
            }
        }

        // Trigger bad ending if countdown reaches 0
        if (!hasGoodEndingTriggered)
        {
            Debug.Log("Countdown finished. Triggering bad ending.");
            SceneManager.LoadScene(badEndingScene);
        }
    }

    public void TriggerGoodEnding()
    {
        if (!hasGoodEndingTriggered)
        {
            Debug.Log("Good ending triggered. Loading good ending scene...");
            hasGoodEndingTriggered = true;

            // Stop the countdown if it's running
            if (isCountdownRunning)
            {
                Debug.Log("Stopping the countdown as good ending is triggered.");
                StopAllCoroutines(); // Stop the countdown coroutine
                countdownText.gameObject.SetActive(false); // Hide the countdown text
                isCountdownRunning = false;
            }

            // Load the good ending scene
            SceneManager.LoadScene(goodEndingScene);
        }
    }
}

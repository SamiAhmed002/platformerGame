using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FinalRobotController : MonoBehaviour
{
    [Header("Robot Settings")]
    public GameObject finalRobot;               // The final robot to appear/disappear
    public string[] dialogueLines;              // Dialogue for the final robot
    public float typewriterSpeed = 0.05f;       // Speed for typewriter effect

    [Header("Countdown Settings")]
    public Text countdownText;                  // UI Text for the countdown timer
    public string goodEndingScene = "GoodEndingCutscene";
    public string badEndingScene = "BadEndingCutscene";
    public int countdownTime = 30;              // Countdown time in seconds

    private bool isCountdownRunning = false;
    private bool hasGoodEndingTriggered = false;

    private void Start()
    {
        finalRobot.SetActive(false); // Hide the robot initially

        // Subscribe to the dialogue finished event
        FinalRobotDialogue finalRobotDialogue = finalRobot.GetComponent<FinalRobotDialogue>();
        if (finalRobotDialogue != null)
        {
            finalRobotDialogue.onDialogueFinished += StartCountdown;
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false); // Hide countdown text initially
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            finalRobot.SetActive(true); // Show the robot when player enters
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            finalRobot.SetActive(false); // Hide the robot when player exits
        }
    }

    private void StartCountdown()
    {
        if (!isCountdownRunning)
        {
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
            countdownText.text = $"00:{currentTime:D2}"; // Update countdown text
            yield return new WaitForSeconds(1f);
            currentTime--;

            if (hasGoodEndingTriggered)
            {
                yield break; // Stop countdown if good ending is triggered
            }
        }

        // Trigger bad ending if countdown reaches 0
        if (!hasGoodEndingTriggered)
        {
            SceneManager.LoadScene(badEndingScene);
        }
    }

    public void TriggerGoodEnding()
    {
        if (!hasGoodEndingTriggered)
        {
            hasGoodEndingTriggered = true;
            SceneManager.LoadScene(goodEndingScene);
        }
    }
}

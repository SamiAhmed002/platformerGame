using System.Collections;
using UnityEngine;

public class TriggerRobotManager : MonoBehaviour
{
    public GameObject robot; // The robot to appear/disappear
    public string[] dialogueLines; // Dialogue for this robot
    public float typewriterSpeed = 0.05f; // Speed for typewriter effect

    private RobotDialogue robotDialogue; // Reference to the RobotDialogue script

    private void Start()
    {
        robot.SetActive(false); // Start with the robot hidden
        robotDialogue = robot.GetComponent<RobotDialogue>();

        // Assign dialogue and typewriter speed
        if (robotDialogue != null)
        {
            robotDialogue.dialogueLines = dialogueLines;
            robotDialogue.typewriterSpeed = typewriterSpeed;
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
}



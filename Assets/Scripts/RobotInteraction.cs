using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RobotInteraction : MonoBehaviour
{
    public Camera playerCamera; // Reference to the player's camera
    public GameObject interactionMessageContainer; // "Press C to interact" UI
    public TextMeshProUGUI interactionText; // Text component for interaction message
    public GameObject subtitlesContainer; // The subtitles container (e.g., SubtitleContainer)
    public TextMeshProUGUI subtitlesText; // The subtitles text component
    public float interactionRange = 10f; // Interaction range
    public KeyCode interactionKey = KeyCode.C; // Interaction key

    private bool isInteracting = false; // Flag to track if interaction is ongoing

    private void Update()
    {
        if (isInteracting)
        {
            // If interaction is ongoing, don't show the interaction message
            interactionMessageContainer.SetActive(false);
            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            // Check if the hit object has a RobotDialogue component
            RobotDialogue robotDialogue = hit.collider.GetComponent<RobotDialogue>();
            if (robotDialogue != null)
            {
                // Show interaction message
                interactionText.text = $"Press {interactionKey} to interact";
                interactionMessageContainer.SetActive(true);

                if (Input.GetKeyDown(interactionKey))
                {
                    interactionMessageContainer.SetActive(false); // Hide the interaction message
                    isInteracting = true; // Set interaction flag
                    robotDialogue.StartInteraction(subtitlesText, subtitlesContainer);
                }
            }
            else
            {
                // Hide interaction message if not hovering over the robot
                interactionMessageContainer.SetActive(false);
            }
        }
        else
        {
            // Hide interaction message if the raycast hits nothing
            interactionMessageContainer.SetActive(false);
        }
    }

    public void EndInteraction()
    {
        // Call this method when the interaction ends
        isInteracting = false;
    }
}






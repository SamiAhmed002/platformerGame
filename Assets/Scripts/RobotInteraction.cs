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

        if (Physics.Raycast(ray, out hit, interactionRange, LayerMask.GetMask("InteractableRobot")))
        {
            // Check if the hit object is a regular robot or the final robot by tag
            if (hit.collider.CompareTag("FinalRobot"))
            {
                // Interaction logic for the final robot
                interactionText.text = $"Press {interactionKey} to interact";
                interactionMessageContainer.SetActive(true);

                if (Input.GetKeyDown(interactionKey))
                {
                    interactionMessageContainer.SetActive(false); // Hide the interaction message
                    isInteracting = true; // Set interaction flag
                    hit.collider.GetComponent<RobotDialogue>().StartInteraction(subtitlesText, subtitlesContainer);
                }
            }
            else
            {
                // Interaction logic for regular robots
                RobotDialogue robotDialogue = hit.collider.GetComponent<RobotDialogue>();
                if (robotDialogue != null)
                {
                    interactionText.text = $"Press {interactionKey} to interact";
                    interactionMessageContainer.SetActive(true);

                    if (Input.GetKeyDown(interactionKey))
                    {
                        interactionMessageContainer.SetActive(false); // Hide the interaction message
                        isInteracting = true; // Set interaction flag
                        robotDialogue.StartInteraction(subtitlesText, subtitlesContainer);
                    }
                }
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
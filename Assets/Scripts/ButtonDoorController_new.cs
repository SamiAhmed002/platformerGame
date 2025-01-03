using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoorController_new : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> buttons = new List<GameObject>();  // List of buttons
    [SerializeField]
    private bool requireAllButtons = false;  // New parameter to require all buttons
    public GameObject door;    

    public float buttonOffset = 0.5f;  
    public float doorOffset = 10f;     
    public float speed = 15f;          

    private List<Vector3> buttonInitialPositions = new List<Vector3>();  // List to store initial button positions
    private Vector3 doorInitialPos;        

    void Start()
    {
        // Validate that at least one button is assigned
        if (buttons.Count == 0)
        {
            Debug.LogError("At least one button must be assigned to ButtonDoorController!");
            enabled = false;
            return;
        }

        // Save the initial positions
        foreach (GameObject button in buttons)
        {
            buttonInitialPositions.Add(button.transform.position);
        }
        doorInitialPos = door.transform.position;
    }

    void Update()
    {
        // Handle button movements independently
        for (int i = 0; i < buttons.Count; i++)
        {
            if (IsPlayerOnButton(buttons[i]))
            {
                buttons[i].transform.position = Vector3.MoveTowards(buttons[i].transform.position, 
                    buttonInitialPositions[i] - new Vector3(0, buttonOffset, 0), speed * Time.deltaTime);
            }
            else
            {
                buttons[i].transform.position = Vector3.MoveTowards(buttons[i].transform.position, 
                    buttonInitialPositions[i], speed * Time.deltaTime);
            }
        }

        // Handle door movement based on button press conditions
        if (IsAnyButtonPressed())
        {
            // Move door upward
            door.transform.position = Vector3.MoveTowards(door.transform.position, 
                doorInitialPos + new Vector3(0, doorOffset, 0), speed * Time.deltaTime);
        }
        else
        {
            // Reset door position
            door.transform.position = Vector3.MoveTowards(door.transform.position, 
                doorInitialPos, speed * Time.deltaTime);
        }
    }

    // Check if any button is being pressed
    private bool IsAnyButtonPressed()
    {
        if (requireAllButtons)
        {
            // Check if ALL buttons are pressed
            foreach (GameObject button in buttons)
            {
                if (!IsPlayerOnButton(button))
                {
                    return false;  // If any button is not pressed, return false
                }
            }
            return true;  // All buttons are pressed
        }
        else
        {
            // Original behavior - check if ANY button is pressed
            foreach (GameObject button in buttons)
            {
                if (IsPlayerOnButton(button))
                {
                    return true;
                }
            }
            return false;
        }
    }

    // Modified to check for either rigidbodies or player
    private bool IsPlayerOnButton(GameObject button)
    {
        Collider buttonCollider = button.GetComponent<Collider>();
        
        // Find all colliders that overlap with the button
        Collider[] overlappingColliders = Physics.OverlapBox(
            buttonCollider.bounds.center, 
            buttonCollider.bounds.extents
        );

        // Check if any of the overlapping objects has a Rigidbody or is the player
        foreach (Collider col in overlappingColliders)
        {
            if (col.attachedRigidbody != null || col.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
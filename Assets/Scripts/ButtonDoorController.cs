using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoorController : MonoBehaviour
{
    public GameObject button;  
    public GameObject door;    
    public GameObject player;  

    public float buttonOffset = 0.5f;  // Button will move down 0.5 units when stood on
    public float doorOffset = 10f;     // Door will move up 10 units when button stood on
    public float speed = 15f;          // Button and Door movement speed

    private Vector3 buttonInitialPos;      // Initial button position
    private Vector3 doorInitialPos;        // Initial door position

    void Start()
    {
        // Save the initial button/door positions for reset
        buttonInitialPos = button.transform.position;
        doorInitialPos = door.transform.position;
    }

    void Update()
    {
        if (IsPlayerOnButton())
        {
            // Move button downward and door upward if player on button
            button.transform.position = Vector3.MoveTowards(button.transform.position, buttonInitialPos - new Vector3(0, buttonOffset, 0), speed * Time.deltaTime);
            door.transform.position = Vector3.MoveTowards(door.transform.position, doorInitialPos + new Vector3(0, doorOffset, 0), speed * Time.deltaTime);
        }
        else
        {
            // Reset button and door positions if player leaves button
            button.transform.position = Vector3.MoveTowards(button.transform.position, buttonInitialPos, speed * Time.deltaTime);
            door.transform.position = Vector3.MoveTowards(door.transform.position, doorInitialPos, speed * Time.deltaTime);
        }
    }

    // Check if player is on button
    private bool IsPlayerOnButton()
    {
        Collider buttonCollider = button.GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();

        return buttonCollider.bounds.Intersects(playerCollider.bounds);
    }
}
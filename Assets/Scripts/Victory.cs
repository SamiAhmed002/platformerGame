using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{

    public GameObject player;

    // Detect when the player collides with enemy
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player and not another object
        if (other.gameObject == player)
        {
            // Send victory message once player touches platform
            Debug.Log("Player has won!");
        }
    }
}

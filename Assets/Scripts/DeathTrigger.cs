using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathTrigger : MonoBehaviour
{

    public GameObject player;
    private Vector3 initialPosition;

    void Start()
    {
        // Track spawn position to teleport player on collision
        initialPosition = player.transform.position;
        //Debug.Log(initialPosition);
    }

    // Detect when the player collides with enemy
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player and not another object
        if (other.gameObject == player)
        {
            // Teleport player back to spawn
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}

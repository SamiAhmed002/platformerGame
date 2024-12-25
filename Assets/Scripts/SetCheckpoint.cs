using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCheckpoint : MonoBehaviour
{
    
    public GameObject player;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == player)
        {
            Debug.Log("checkpoint hit");
            PlayerMovement movementScript = other.GetComponent<PlayerMovement>();
            movementScript.SetSpawn(transform.position);
        }
    }
}

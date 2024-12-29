using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCheckpoint : MonoBehaviour
{
    
    public GameObject player;
    public GameObject plane1;
    public GameObject plane2;

    void Start() {
        plane1.GetComponent<Renderer>().material.color = Color.red;
        plane2.GetComponent<Renderer>().material.color = Color.red;
        if (gameObject.tag == "Easypoint" && SettingsManager.gameMode != 0) {
            gameObject.SetActive(false);
        }
        else if (gameObject.tag == "Midpoint" && SettingsManager.gameMode == 2) {
            gameObject.SetActive(false);
        }
        else {
            gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == player) {
            Debug.Log("checkpoint hit");
            PlayerMovement movementScript = other.GetComponent<PlayerMovement>();
            movementScript.SetSpawn(transform.position);
            plane1.GetComponent<Renderer>().material.color = Color.green;
            plane2.GetComponent<Renderer>().material.color = Color.green;
        }
    }
}

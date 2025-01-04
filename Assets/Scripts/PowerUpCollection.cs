using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollection : MonoBehaviour
{

    public int ID;
    public GameObject player;

    void Update() {
        if ((this.ID == 2 && SpawnLocation.hasLaser) || (this.ID == 3 && SpawnLocation.hasLevitation)) {
                Destroy(gameObject);
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player and not another object
        if (other.gameObject == player)
        {
            if (this.ID == 2) {
                SpawnLocation.hasLaser = true;
            }
            else if (this.ID == 3) {
                SpawnLocation.hasLevitation = true;
            }         
        }
    }
}

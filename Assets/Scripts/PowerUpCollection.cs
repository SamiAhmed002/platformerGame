using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollection : MonoBehaviour
{

    public int ID;
    public GameObject player;
    public GameObject inventory;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player and not another object
        if (other.gameObject == player)
        {
            inventory.SetActive(true);
            InventorySelect[] buttons = FindObjectsByType<InventorySelect>(FindObjectsSortMode.None);
            foreach (InventorySelect button in buttons)
            {
                Debug.Log(button);
                if (button.ID == this.ID)
                {
                    button.UnlockButton();
                    break;
                }
            }
            inventory.SetActive(false);

            Destroy(gameObject);            
        }
    }
}

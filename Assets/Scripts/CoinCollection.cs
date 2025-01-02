using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCollection : MonoBehaviour
{
    public GameObject player;
    public InventorySelect inventorySelect; // Reference to the InventorySelect script
    public TextMeshProUGUI coinCounterText;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.gameObject == player)
        {
                inventorySelect.coinCount += 1;
                coinCounterText.text = "x" + inventorySelect.coinCount;

            // Destroy the coin object
            Destroy(gameObject);
        }
    }
}

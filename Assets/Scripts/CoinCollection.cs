using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCollection : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI coinCounterText;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.gameObject == player)
        {
                SpawnLocation.coins += 1;
                coinCounterText.text = "x" + SpawnLocation.coins;

            // Destroy the coin object
            Destroy(gameObject);
        }
    }
}

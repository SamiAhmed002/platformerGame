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
        // Check if it's the player touching the trigger
        if (other.gameObject == player)
        {
            // Update coin count
            SpawnLocation.coins += 1;
            coinCounterText.text = "x" + SpawnLocation.coins;

            // Destroy coin
            Destroy(gameObject);
        }
    }
}

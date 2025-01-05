using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDUpdate : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    // Start is called before the first frame update
    void Start()
    {
        // Update coin counter in HUD
        coinText.text = "x" + SpawnLocation.coins;
    }

}

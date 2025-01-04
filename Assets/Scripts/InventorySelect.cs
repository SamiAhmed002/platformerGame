using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InventorySelect : MonoBehaviour
{

    public int ID;
    public string itemName;
    public TextMeshProUGUI itemText;
    public static InventorySelect currentSelection;
    public GameObject inventory;
    public RawImage powerIcon;
    public Texture itemImage;
    private Button button;
    public TextMeshProUGUI coinText;

    void Start()
    {
        button = GetComponent<Button>();
        if (this.ID == 1)
        {
            button.interactable = true;
            Selected();
        }
        else if ((this.ID == 2 && SpawnLocation.hasLaser) || (this.ID == 3 && SpawnLocation.hasLevitation)) {
            button.interactable = true;
        }
        else {
            button.interactable = false;
        }
    }

    void Update() {
        if (this.ID == 4) {
            coinText.text = SpawnLocation.coins + "/5";
            if (SpawnLocation.coins >= 5) {
                button.interactable = true;
            }
            else {
                button.interactable = false;
            }
        }
        if ((this.ID == 2 && SpawnLocation.hasLaser) || (this.ID == 3 && SpawnLocation.hasLevitation)) {
            button.interactable = true;
        }
    }

    public void Selected() {
        
        currentSelection = this;
        itemText.text = itemName;
        powerIcon.texture = itemImage;
        inventory.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void HoverEnter() {
        if (button.interactable == true) {
            itemText.text = itemName;
        }
    }

    public void HoverExit() {
        itemText.text = currentSelection.itemName;
    }

}

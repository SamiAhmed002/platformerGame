using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class SpawnLocation {
    public static Vector3 spawnPosition = Vector3.zero; 
}

public static class SettingsManager {
    public static float sensitivityValue = 5f;
}

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject playMenu;
    public GameObject settingsMenu;
    public Slider sensitivity;

    void Start() {
        // Lock the cursor and make it invisible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ensure the pause menu is hidden at the start
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);
        sensitivity.value = SettingsManager.sensitivityValue;
        sensitivity.onValueChanged.AddListener(OnSensitivityChanged);
    }

    public void PlayButton() {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void OpenSettings() {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void BackButton() {
        settingsMenu.SetActive(false);
        playMenu.SetActive(false);  
        mainMenu.SetActive(true);
    }

    public void LoadTutorial() {
        SpawnLocation.spawnPosition = new Vector3(127,6,0);
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadLevel1() {
        SpawnLocation.spawnPosition = new Vector3(-200,12,0);
        SceneManager.LoadScene("SampleScene");
    }

    public void OnSensitivityChanged(float value)
    {
        SettingsManager.sensitivityValue = value;
    }
}

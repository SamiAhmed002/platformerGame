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
    public static int gameMode = 0;
}

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject playMenu;
    public GameObject levelMenu;
    public GameObject settingsMenu;
    public Slider sensitivity;
    public Toggle easyModeToggle;
    public Toggle mediumModeToggle;
    public Toggle hardModeToggle;

    void Start() {
        // Lock the cursor and make it invisible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ensure the pause menu is hidden at the start
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        levelMenu.SetActive(false);
        settingsMenu.SetActive(false);
        sensitivity.value = SettingsManager.sensitivityValue;
        sensitivity.onValueChanged.AddListener(OnSensitivityChanged);
        
        UpdateToggles();

        // Add listeners for toggles
        easyModeToggle.onValueChanged.AddListener(OnEasyModeChanged);
        mediumModeToggle.onValueChanged.AddListener(OnMediumModeChanged);        
        hardModeToggle.onValueChanged.AddListener(OnHardModeChanged);

    }

    public void PlayButton() {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void OpenSettings() {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ContinueButton() {
        playMenu.SetActive(false);
        levelMenu.SetActive(true);
    }

    public void BackButton() {
        settingsMenu.SetActive(false);
        playMenu.SetActive(false);  
        mainMenu.SetActive(true);
    }

    public void LevelBackButton() {
        levelMenu.SetActive(false);
        playMenu.SetActive(true);
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


    void UpdateToggles() {
        easyModeToggle.isOn = (SettingsManager.gameMode == 0);
        mediumModeToggle.isOn = (SettingsManager.gameMode == 1);
        hardModeToggle.isOn = (SettingsManager.gameMode == 2);
    }

    void OnEasyModeChanged(bool isOn)
    {
        if (isOn) {
            SettingsManager.gameMode = 0;
            UpdateToggles();
        }
    }

    void OnMediumModeChanged(bool isOn)
    {
        if (isOn) {
            SettingsManager.gameMode = 1;
            UpdateToggles();
        }
    }


    void OnHardModeChanged(bool isOn)
    {
       if (isOn) {
            SettingsManager.gameMode = 2;
            UpdateToggles();
        }
    }
}

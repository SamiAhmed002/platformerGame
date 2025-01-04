using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class SpawnLocation {
    public static Vector3 spawnPosition = new Vector3(130, 8, 0); 
    public static int coins = 0;
    public static bool hasLaser = false;
    public static bool hasLevitation = false;
}

public static class SettingsManager {
    public static float sensitivityValue = 5f;
    public static int gameMode = 0;
    public static int progress = 0; //increments by 1 per level unlocked, starting with 0 for tutorial
    //set progress to 2 to unlock all levels upon starting game
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start on correct screen
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        levelMenu.SetActive(false);
        settingsMenu.SetActive(false);
        sensitivity.value = SettingsManager.sensitivityValue;
        sensitivity.onValueChanged.AddListener(OnSensitivityChanged);
        
        //Easy/medium/hard toggles to only have 1 selected at a time
        UpdateToggles();

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


    public static class TutorialManager
    {
        public static bool hasPlayedCutScene = false;
    }

    public void LoadTutorial()
    {
        if (!TutorialManager.hasPlayedCutScene)
        {
            // Set the flag to indicate the cut-scene is being played
            TutorialManager.hasPlayedCutScene = true;

            // Load the cut-scene
            SceneManager.LoadScene("IntroCutScene");
        }
        else
        {
            // Directly load the tutorial level
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void LoadLevel1() {
        SpawnLocation.spawnPosition = new Vector3(-200,12,0);
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadLevel2() {
        SpawnLocation.spawnPosition = new Vector3(-873,62,-4);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    void Start() {
        // Ensure the game starts unpaused
        Time.timeScale = 1; 
        isPaused = false;

        // Lock the cursor and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ensure the pause menu is hidden at the start
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel")) {
            if(!isPaused) {
                Pause();
            }
            else {
                Resume();
            }
        }
        
    }

    void Resume() {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(false); 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        Time.timeScale = 1;
    }

    void Pause() {
        Time.timeScale = 0;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame() {
        Resume();
    }

    public void RestartLevel() {
        Resume();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OpenSettings() {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitToMenu() {
        Resume();
        SceneManager.LoadScene(1);
    }

    public void BackButton() {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}

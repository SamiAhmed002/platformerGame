using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameOnFadeOut : MonoBehaviour
{
    public string nextSceneName = "MainMenu"; // Set the name of the scene to load after the fade-out

    // Function to be called by the animation event
    public void EndGame()
    {
        Debug.Log("Fade-out complete. Ending game...");
        SceneManager.LoadScene(nextSceneName); // Load the specified scene
    }
}

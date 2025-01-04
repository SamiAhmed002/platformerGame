using UnityEngine;

public class SubtitleTrigger : MonoBehaviour
{
    public SubtitleManager subtitleManager;
    public AudioSource robotAudioSource; // Drag the robot's AudioSource here

    void Start()
    {
        string[] lines = {
            "The main spaceship is under attack.",
            "Restore the engine before it leaves orbit.",
            "If you don't, it will burn up in the atmosphere!"
        };

        // Match these times to the timestamps in your audio file (in seconds)
        float[] startTimes = { 0f, 2f, 5f };

        // Start the audio and display subtitles
        robotAudioSource.Play();
        subtitleManager.DisplaySubtitlesWithAudio(lines, startTimes, robotAudioSource);
    }
}

using UnityEngine;
using System.Collections;

public class RedLightGreenLightController : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject watcher;          // The object that turns around
    public GameObject player;           // The player to detect
    public GameObject[] doors;          // Array of doors to control
    public GameObject deactivationButton;  // Button to permanently stop the watcher

    [Header("Indicator Lights")]
    public Light[] redLights;    // Array of spotlights that turn on during red light
    public Light[] greenLights;  // Array of spotlights that turn on during green light
    public float lightIntensity = 1f;  // How bright the lights should be when on

    [Header("Audio")]
    public AudioClip redLightSound;     // Sound played when turning to watch players
    public AudioClip greenLightSound;   // Sound played when turning away from players
    public AudioClip deactivationSound; // Sound played when watcher is deactivated
    [Range(0f, 1f)]
    public float soundVolume = 1f;      // Control volume of the sounds
    public bool playSound = true;       // Toggle sounds on/off
    private AudioSource audioSource;     // Component to play the sounds

    [Header("Detection Settings")]
    public float detectionRange = 20f;  
    public bool showDetectionGizmo = true;
    public bool detectCameraMovement = true;  // Toggle for camera movement detection
    public float cameraRotationThreshold = 0.1f;  // How sensitive the camera rotation detection is
    public Camera playerCamera;  // Reference to the player's camera

    [Header("Movement Settings")]
    public float rotationSpeed = 180f;  // How fast the watcher turns
    public float doorOffset = 10f;      // How far doors move up when triggered
    public float doorSpeed = 15f;       // Door movement speed
    
    [Header("Timing Settings")]
    public float minWatchTime = 2f;     // Minimum time watching players
    public float maxWatchTime = 5f;     // Maximum time watching players
    public float minWaitTime = 3f;      // Minimum time showing back to players
    public float maxWaitTime = 7f;      // Maximum time showing back to players

    private Vector3[] doorInitialPositions;  // Store initial door positions
    private Vector3 lastPlayerPosition;      // To track player movement
    private bool isWatching = false;         // Is the watcher facing players?
    private bool isRotating = false;         // Is the watcher currently turning?
    private bool[] doorsTriggered;  // Track which doors are triggered
    private bool gameActive = false;    // Tracks if the game is active
    private Coroutine watcherRoutine;   // Reference to the watcher routine
    private Quaternion lastCameraRotation;  // To track camera rotation
    private bool permanentlyDeactivated = false;  // Tracks if watcher is permanently stopped
    private bool buttonPressed = false;  // Prevents multiple triggers of the button

    [Header("Button Settings")]
    public float buttonOffset = 0.5f;    // How far the button moves down when pressed
    public float buttonSpeed = 5f;       // How fast the button moves
    private Vector3 buttonInitialPos;    // Store initial button position
    private bool isButtonMoving = false; // Prevent multiple button press animations

    void Start()
    {
        // Initialize audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        doorInitialPositions = new Vector3[doors.Length];
        doorsTriggered = new bool[doors.Length];
        for (int i = 0; i < doors.Length; i++)
        {
            doorInitialPositions[i] = doors[i].transform.position;
            doorsTriggered[i] = false;
        }

        // Initialize all green lights to off at start
        foreach (Light light in greenLights)
        {
            if (light != null)
            {
                light.intensity = 0f;
            }
        }

        lastPlayerPosition = player.transform.position;
        lastCameraRotation = playerCamera.transform.rotation;
        // Don't start the watcher routine immediately

        // If playerCamera isn't assigned, try to find it
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Store initial button position
        if (deactivationButton != null)
        {
            buttonInitialPos = deactivationButton.transform.position;
        }
    }

    void Update()
    {
        // Don't process anything if permanently deactivated
        if (permanentlyDeactivated) return;

        // Check for button press
        if (deactivationButton != null && !buttonPressed && IsPlayerNearButton())
        {
            DeactivateWatcherPermanently();
            buttonPressed = true;
            return;
        }

        // Check if player has entered the detection zone
        if (!gameActive && IsPlayerInRange())
        {
            StartGame();
        }
        // Check if player has left the detection zone
        else if (gameActive && !IsPlayerInRange())
        {
            StopGame();
        }

        if (gameActive)
        {
            if (isWatching && !isRotating)
            {
                if (HasPlayerMoved() || (detectCameraMovement && HasCameraMoved()))
                {
                    TriggerDoors();
                }
            }
            else if (!isWatching && !isRotating)
            {
                ResetDoors();
            }
        }

        // Update last known positions
        lastPlayerPosition = player.transform.position;
        lastCameraRotation = playerCamera.transform.rotation;
    }

    private bool HasPlayerMoved()
    {
        // Check if player position changed more than a tiny threshold
        return Vector3.Distance(player.transform.position, lastPlayerPosition) > 0.01f;
    }

    private bool IsPlayerInRange()
    {
        return Vector3.Distance(watcher.transform.position, player.transform.position) <= detectionRange;
    }

    private void TriggerDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (!doorsTriggered[i])  // Only trigger if not already triggered
            {
                StartCoroutine(MoveDoorUp(i));
                doorsTriggered[i] = true;
            }
        }
    }

    private void ResetDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doorsTriggered[i])  // Only reset if currently triggered
            {
                StartCoroutine(MoveDoorDown(i));
                doorsTriggered[i] = false;
            }
        }
    }

    private IEnumerator MoveDoorUp(int doorIndex)
    {
        // Calculate target position based on offset direction
        Vector3 targetPosition = doorInitialPositions[doorIndex] + new Vector3(0, doorOffset, 0);
        
        // Move door until it reaches target position, regardless of direction
        while (Vector3.Distance(doors[doorIndex].transform.position, targetPosition) > 0.01f)
        {
            doors[doorIndex].transform.position = Vector3.MoveTowards(
                doors[doorIndex].transform.position,
                targetPosition,
                doorSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        // Ensure door is exactly at target position
        doors[doorIndex].transform.position = targetPosition;
    }

    private IEnumerator MoveDoorDown(int doorIndex)
    {
        // Move door until it reaches initial position
        while (Vector3.Distance(doors[doorIndex].transform.position, doorInitialPositions[doorIndex]) > 0.01f)
        {
            doors[doorIndex].transform.position = Vector3.MoveTowards(
                doors[doorIndex].transform.position,
                doorInitialPositions[doorIndex],
                doorSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        // Ensure door is exactly at initial position
        doors[doorIndex].transform.position = doorInitialPositions[doorIndex];
    }

    // Add visualization of detection range in the Unity Editor
    void OnDrawGizmos()
    {
        if (showDetectionGizmo && watcher != null)
        {
            // Show detection zone in red when inactive, yellow when active, not at all when deactivated
            if (!permanentlyDeactivated)
            {
                Gizmos.color = gameActive ? Color.yellow : Color.red;
                Gizmos.DrawWireSphere(watcher.transform.position, detectionRange);
            }
        }

        // Show button interaction range
        if (deactivationButton != null && showDetectionGizmo)
        {
            Gizmos.color = buttonPressed ? Color.gray : Color.green;
            Gizmos.DrawWireSphere(deactivationButton.transform.position, 2f);
        }
    }

    private void StartGame()
    {
        gameActive = true;
        // Start the watcher routine
        watcherRoutine = StartCoroutine(WatcherRoutine());
    }

    private void StopGame()
    {
        gameActive = false;
        if (watcherRoutine != null)
        {
            StopCoroutine(watcherRoutine);
            watcherRoutine = null;
        }
        
        watcher.transform.rotation = Quaternion.identity;
        ResetDoors();
        TurnOffAllLights();  // Turn off all lights when game stops
        
        isWatching = false;
        isRotating = false;
    }

    private IEnumerator WatcherRoutine()
    {
        // Initial delay before starting
        yield return new WaitForSeconds(2f);

        while (gameActive)
        {
            // Show back to players (Green Light)
            yield return StartCoroutine(RotateWatcher(180f));
            isWatching = false;
            UpdateLights(false);  // Turn on green lights
            if (playSound && greenLightSound != null)
            {
                audioSource.clip = greenLightSound;
                audioSource.volume = soundVolume;
                audioSource.Play();
            }
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

            // Turn to watch players (Red Light)
            yield return StartCoroutine(RotateWatcher(180f));
            isWatching = true;
            UpdateLights(true);  // Turn on red lights
            if (playSound && redLightSound != null)
            {
                audioSource.clip = redLightSound;
                audioSource.volume = soundVolume;
                audioSource.Play();
            }
            yield return new WaitForSeconds(Random.Range(minWatchTime, maxWatchTime));
        }
    }

    private IEnumerator RotateWatcher(float angle)
    {
        isRotating = true;
        Quaternion startRotation = watcher.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, angle, 0);
        float elapsedTime = 0f;
        float rotationTime = angle / rotationSpeed;

        while (elapsedTime < rotationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationTime;
            watcher.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        watcher.transform.rotation = targetRotation;
        isRotating = false;
    }

    private bool HasCameraMoved()
    {
        if (playerCamera == null || SettingsManager.gameMode < 2) return false;

        // Check how much the camera has rotated
        float rotationDifference = Quaternion.Angle(playerCamera.transform.rotation, lastCameraRotation);
        return rotationDifference > cameraRotationThreshold;
    }

    private bool IsPlayerNearButton()
    {
        if (deactivationButton == null) return false;
        
        float buttonRange = 2f; // How close player needs to be to the button
        return Vector3.Distance(deactivationButton.transform.position, player.transform.position) <= buttonRange;
    }

    private void DeactivateWatcherPermanently()
    {
        if (isButtonMoving) return;

        permanentlyDeactivated = true;
        StopGame();  // This will also turn off all lights
        
        if (deactivationButton != null)
        {
            StartCoroutine(PressButton());
        }
    }

    private IEnumerator PressButton()
    {
        isButtonMoving = true;

        // Move button down
        Vector3 pressedPosition = buttonInitialPos - new Vector3(0, buttonOffset, 0);
        while (Vector3.Distance(deactivationButton.transform.position, pressedPosition) > 0.01f)
        {
            deactivationButton.transform.position = Vector3.MoveTowards(
                deactivationButton.transform.position,
                pressedPosition,
                buttonSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Play deactivation sound
        if (playSound && audioSource != null)
        {
            if (deactivationSound != null)
            {
                audioSource.PlayOneShot(deactivationSound);
            }
            else
            {
                audioSource.PlayOneShot(greenLightSound); // Fallback if no deactivation sound
            }
        }

        // Optional: Wait a moment while button is pressed
        yield return new WaitForSeconds(0.5f);

        // Move button back up
        while (Vector3.Distance(deactivationButton.transform.position, buttonInitialPos) > 0.01f)
        {
            deactivationButton.transform.position = Vector3.MoveTowards(
                deactivationButton.transform.position,
                buttonInitialPos,
                buttonSpeed * Time.deltaTime
            );
            yield return null;
        }

        isButtonMoving = false;

        // Change watcher appearance after button press
        if (watcher != null)
        {
            watcher.transform.rotation = Quaternion.identity;
            
            Renderer watcherRenderer = watcher.GetComponent<Renderer>();
            if (watcherRenderer != null)
            {
                watcherRenderer.material.color = Color.gray;
            }
        }
    }

    private void UpdateLights(bool isRedLight)
    {
        // Handle red lights - turn on during red light, off during green light
        foreach (Light light in redLights)
        {
            if (light != null)
            {
                // Only modify intensity, preserve existing color
                light.intensity = isRedLight ? lightIntensity : 0f;
            }
        }

        // Handle green lights - turn on during green light, off during red light
        foreach (Light light in greenLights)
        {
            if (light != null)
            {
                // Only modify intensity, preserve existing color
                light.intensity = isRedLight ? 0f : lightIntensity;
            }
        }
    }

    private void TurnOffAllLights()
    {
        // Turn off red lights
        foreach (Light light in redLights)
        {
            if (light != null)
            {
                light.intensity = 0f;
            }
        }

        // Turn off green lights
        foreach (Light light in greenLights)
        {
            if (light != null)
            {
                light.intensity = 0f;
            }
        }
    }
} 
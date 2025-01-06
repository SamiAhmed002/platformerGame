using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;           // Reference to the moving door (visual part)
    public float openHeight = 5f;    // How far the door moves upward to open
    public float doorSpeed = 2f;
    private Vector3 initialPosition;
    private bool doorLocked = false; // Prevents reopening once locked
    private bool isMoving = false;   // Prevents multiple coroutines

    [Header("Sound Settings")]
    public AudioClip doorOpenSound;  // Sound to play when the door opens
    private AudioSource audioSource; // AudioSource to play the sound

    private void Start()
    {
        // Store the initial position of the door
        initialPosition = door.position;

        // Get or Add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            // Set default AudioSource properties
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // Make it fully 3D sound
            audioSource.volume = 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Open door for player when trigger touched
        if (!doorLocked && other.CompareTag("Player"))
        {
            Debug.Log("Player entered: Opening door...");
            if (!isMoving) StartCoroutine(OpenDoor());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Close door behind player when they leave trigger
        if (!doorLocked && other.CompareTag("Player"))
        {
            Debug.Log("Player exited: Closing door...");
            if (!isMoving) StartCoroutine(CloseDoor());
        }
    }

    private System.Collections.IEnumerator OpenDoor()
    {
        isMoving = true;
        Vector3 targetPosition = initialPosition + Vector3.up * openHeight;

        // Play door open sound
        if (doorOpenSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }

        // Smoothly open the door
        while (Vector3.Distance(door.position, targetPosition) > 0.01f)
        {
            door.position = Vector3.MoveTowards(door.position, targetPosition, doorSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
        Debug.Log("Door fully opened.");
    }

    private System.Collections.IEnumerator CloseDoor()
    {
        isMoving = true;
        Vector3 targetPosition = initialPosition;

        // Smoothly close the door
        while (Vector3.Distance(door.position, targetPosition) > 0.01f)
        {
            door.position = Vector3.MoveTowards(door.position, targetPosition, doorSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
        doorLocked = true; // Lock the door permanently after closing
        Debug.Log("Door closed and locked.");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public Slider sensitivity;

    // Audio
    public AudioSource movementAudioSource;  // AudioSource for movement sounds
    public AudioClip walkSound;             // Audio clip for walking
    public AudioClip runSound;              // Audio clip for running
    public AudioClip jumpSound;             // Audio clip for jumping

    // Player movement settings
    public float walkSpeed = 6f;          // Walking speed of the player.
    public float runSpeed = 12f;          // Running speed of the player.
    public float jumpPower = 7f;          // Force applied when jumping.
    public float gravity = 10f;           // Gravity affecting the player when not grounded.
    public float lookSpeed;               // Sensitivity for looking around (mouse movement).
    public float lookXLimit = 75f;        // Limit to the vertical looking angle.
    public float defaultHeight = 2f;      // Default height of the player (standing height).

    private Vector3 moveDirection = Vector3.zero;  // Stores the player's movement direction.
    private float rotationX = 0;                   // Tracks the vertical rotation angle for looking up/down.
    private CharacterController characterController; // Reference to the CharacterController component.
    private bool canMove = true;         // Flag to check if the player can move.
    private bool isPlayingMovementSound = false;  // To track whether the movement sound is already playing
    public Vector3 spawnPosition;

    public bool slowPowerActive = false;

    void Start()
    {
        // Initialize the CharacterController component and lock the cursor to the game window.
        characterController = GetComponent<CharacterController>();
        characterController.enabled = false;

        // Set the spawn position and enable the controller.
        spawnPosition = SpawnLocation.spawnPosition;
        transform.position = SpawnLocation.spawnPosition; //Comment for debugging 
        characterController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;  // Lock the mouse cursor to the center of the screen.
        Cursor.visible = false;                   // Hide the cursor while playing.

        // Ensure AudioSource is properly configured
        if (movementAudioSource == null)
        {
            movementAudioSource = gameObject.AddComponent<AudioSource>();
            movementAudioSource.loop = true;
        }
    }

    void Update()
    {
        // Movement: Get the forward and right vectors based on the player's current rotation.
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Check if the player is holding the run key (Left Shift).
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Calculate movement speed based on whether the player is running or walking.
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;  // Forward/backward movement.
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0; // Left/right movement.

        // Store the current Y-axis movement (e.g., gravity or jumping) so it can be reapplied.
        float movementDirectionY = moveDirection.y;

        // Calculate the overall movement direction using forward and right vectors.
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Play movement sound based on player input
        HandleMovementSound(curSpeedX, curSpeedY, isRunning);

        // Jumping logic: If the player is grounded and presses the jump button (space), apply the jump power.
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;

            // Play the jump sound
            if (jumpSound != null)
            {
                movementAudioSource.PlayOneShot(jumpSound);
            }
        }
        else
        {
            // Retain the previous Y-axis movement (gravity, fall, etc.).
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity when the player is not grounded.
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the character using the calculated movement direction.
        characterController.Move(moveDirection * Time.deltaTime);

        // Looking around (camera movement):
        if (canMove)
        {
            // Vertical rotation (looking up and down) with clamping to limit the rotation.
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            // Apply the vertical rotation to the player camera.
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Horizontal rotation (looking left and right).
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Adjust look speed dynamically based on sensitivity.
        lookSpeed = 2f * sensitivity.value * Time.timeScale / (slowPowerActive ? 0.5f : 1);

        gravity = 10f / (slowPowerActive ? 0.5f : 1);
        jumpPower = 7f / (slowPowerActive ? 0.5f : 1);
    }

    void HandleMovementSound(float curSpeedX, float curSpeedY, bool isRunning)
    {
        bool isMoving = curSpeedX != 0 || curSpeedY != 0;

        if (isMoving && characterController.isGrounded)
        {
            if (!isPlayingMovementSound)
            {
                movementAudioSource.clip = isRunning ? runSound : walkSound;
                movementAudioSource.Play();
                isPlayingMovementSound = true;
            }
            else if (movementAudioSource.clip != (isRunning ? runSound : walkSound))
            {
                movementAudioSource.clip = isRunning ? runSound : walkSound;
                movementAudioSource.Play();
            }
        }
        else if (isPlayingMovementSound)
        {
            movementAudioSource.Stop();
            isPlayingMovementSound = false;
        }
    }

    public void ApplyVerticalLift(float liftSpeed)
    {
        moveDirection.y = liftSpeed;
        Debug.Log("Lift applied: " + liftSpeed);
    }

    public void SetSpawn(Vector3 checkpoint)
    {
        SpawnLocation.spawnPosition = checkpoint;
    }

    public void Respawn()
    {
        Debug.Log("Respawning to spawn position...");
        characterController.enabled = false;
        transform.position = spawnPosition;
        Debug.Log("Spawn position set to " + spawnPosition);
        characterController.enabled = true;
    }
}

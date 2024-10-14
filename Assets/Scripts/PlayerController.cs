using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jump;
    public CharacterController controller;
    public float gravity;
    public float rotation;
    private float mouseX;
    public Vector3 direction;
    private Vector3 verticalMovement;

    public Transform cameraTransform; // Reference to the camera's transform

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the forward and right direction based on the camera's orientation
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten the camera's forward and right directions to avoid unintended vertical movement
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the forward and right directions
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Get input for movement along the camera's forward and right
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;

        // Calculate the movement direction relative to the camera's orientation
        direction = (cameraForward * moveZ) + (cameraRight * moveX);

        // Handle jumping
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalMovement.y = jump;
        }

        // Apply gravity
        verticalMovement.y += Physics.gravity.y * gravity * Time.deltaTime;

        // Move the player based on direction and vertical movement
        controller.Move((direction + new Vector3(0, verticalMovement.y, 0)) * Time.deltaTime);

        // Handle rotation of the player with mouse movement
        mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotation * Time.deltaTime);
    }
}

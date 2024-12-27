using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    public GameObject portalPrefabA;  // Prefab for Portal A
    public GameObject portalPrefabB;  // Prefab for Portal B
    public Camera playerCamera;       // Reference to player's camera
    public float maxDistance = 500f;  // Maximum range of portal shooting
    
    [Header("Levitation Settings")]
    public float levitationDistance = 3f;    // Distance at which object floats from player
    public float levitationSpeed = 8f;       // Speed at which object moves to levitation position
    public float levitationRange = 20f;      // Maximum range to pick up objects

    [Header("Launch Settings")]
    public float launchForce = 2000f;  // Force applied when launching object

    private GameObject portalA = null;        // Instance of Portal A
    private GameObject portalB = null;        // Instance of Portal B
    private GameObject levitatedObject = null;
    private Rigidbody levitatedRigidbody = null;
    private Collider playerCollider;    // Reference to player's collider
    private Collider levitatedCollider; // Reference to levitated object's collider
    private Animator levitatedAnimator = null;

    void Start()
    {
        // Get the player's collider (assuming PortalGun is attached to the player)
        playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
        {
            // If the collider is on the parent object
            playerCollider = GetComponentInParent<Collider>();
        }
    }

    void Update()
    {
        // Right-click to shoot a portal
        if (Input.GetMouseButtonDown(1))
        {
            ShootPortal();
        }

        // Left-click to start levitation
        if (Input.GetMouseButtonDown(0))
        {
            TryStartLevitation();
        }
        // Release left-click to drop object
        else if (Input.GetMouseButtonUp(0))
        {
            StopLevitation();
        }

        // Launch object with F while levitating
        if (Input.GetKeyDown(KeyCode.F) && levitatedObject != null)
        {
            LaunchObject();
        }

        // Update levitated object position while holding
        if (levitatedObject != null)
        {
            UpdateLevitatedObjectPosition();
        }
    }

    void TryStartLevitation()
    {
        if (levitatedObject != null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, levitationRange))
        {
            if (hit.collider.CompareTag("Floating"))
            {
                levitatedObject = hit.collider.gameObject;
                levitatedRigidbody = levitatedObject.GetComponent<Rigidbody>();
                levitatedCollider = hit.collider;
                
                levitatedAnimator = levitatedObject.GetComponent<Animator>();
                if (levitatedAnimator != null)
                {
                    levitatedAnimator.enabled = false;
                }
                
                if (levitatedRigidbody != null && levitatedCollider != null && playerCollider != null)
                {
                    Physics.IgnoreCollision(playerCollider, levitatedCollider, true);
                    levitatedRigidbody.useGravity = false;
                    levitatedRigidbody.drag = 10f;
                }
            }
        }
    }

    void StopLevitation()
    {
        if (levitatedObject != null && levitatedRigidbody != null)
        {
            // Re-enable collision between player and object
            if (levitatedCollider != null && playerCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, levitatedCollider, false);
            }

            // Add these lines to re-enable the animator
            if (levitatedAnimator != null)
            {
                levitatedAnimator.enabled = true;
            }

            levitatedRigidbody.useGravity = true;
            levitatedRigidbody.drag = 0f;
            levitatedObject = null;
            levitatedRigidbody = null;
            levitatedCollider = null;
            levitatedAnimator = null;
        }
    }

    void UpdateLevitatedObjectPosition()
    {
        // Calculate desired position in front of the player
        Vector3 targetPosition = playerCamera.transform.position + 
                               playerCamera.transform.forward * levitationDistance;

        // Smoothly move the object to the target position
        if (levitatedRigidbody != null)
        {
            Vector3 direction = targetPosition - levitatedRigidbody.position;
            levitatedRigidbody.velocity = direction * levitationSpeed;
        }
    }

    void ShootPortal()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            // Don't create portals on objects tagged as Portal or Floating
            if (!hit.collider.CompareTag("Portal") && !hit.collider.CompareTag("Enemy") && !hit.collider.CompareTag("Non-Portal"))
            {
                // If both portals are active, destroy them before placing new ones
                if (portalA != null && portalB != null)
                {
                    Destroy(portalA);
                    Destroy(portalB);
                    portalA = null;
                    portalB = null;
                }

                // Place the first portal (Portal A) if none exist
                if (portalA == null)
                {
                    Vector3 portalPosition = hit.point + hit.normal * 0.05f;
                    portalA = Instantiate(portalPrefabA, portalPosition, Quaternion.LookRotation(hit.normal));
                    Debug.Log("Portal A placed at: " + hit.point);
                }
                // Place the second portal (Portal B) if Portal A exists
                else if (portalB == null)
                {
                    Vector3 portalPosition = hit.point + hit.normal * 0.05f;
                    portalB = Instantiate(portalPrefabB, portalPosition, Quaternion.LookRotation(hit.normal));
                    Debug.Log("Portal B placed at: " + hit.point);

                    // Link the two portals
                    portalA.GetComponent<Portal>().linkedPortal = portalB;
                    portalB.GetComponent<Portal>().linkedPortal = portalA;
                }
            }
        }
    }

    void LaunchObject()
    {
        if (levitatedObject != null && levitatedRigidbody != null)
        {
            // Re-enable collision between player and object
            if (levitatedCollider != null && playerCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, levitatedCollider, false);
            }

            // Add these lines to re-enable the animator
            if (levitatedAnimator != null)
            {
                levitatedAnimator.enabled = true;
            }

            // Apply launch force in the direction player is facing
            levitatedRigidbody.velocity = Vector3.zero;
            levitatedRigidbody.AddForce(playerCamera.transform.forward * launchForce);

            // Reset object state
            levitatedRigidbody.useGravity = true;
            levitatedRigidbody.drag = 0f;
            levitatedObject = null;
            levitatedRigidbody = null;
            levitatedCollider = null;
            levitatedAnimator = null;
        }
    }
}

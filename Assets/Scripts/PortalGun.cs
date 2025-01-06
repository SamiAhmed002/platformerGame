using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PortalGun : MonoBehaviour
{
    public GameObject portalPrefabA;  // Prefab for Portal A
    public GameObject portalPrefabB;  // Prefab for Portal B
    public Camera playerCamera;       // Reference to player's camera
    public float maxDistance = 500f;  // Maximum range of portal shooting

    [Header("Levitation Settings")]
    public float levitationDistance = 3f;    // Default distance for regular objects
    public float robotSphereLevitationDistance = 6f; // Distance for RobotSphere objects
    public float levitationSpeed = 8f;       // Speed at which object moves to levitation position
    public float levitationRange = 20f;      // Maximum range to pick up objects

    [Header("Launch Settings")]
    public float launchForce = 2000f;  // Force applied when launching object

    private GameObject portalA = null;  // Instance of Portal A
    private GameObject portalB = null;  // Instance of Portal B

    public PauseGame pauseGame;
    public InventorySelect inventorySelect;
    public TextMeshProUGUI coinCounterText;
    public ParticleSystem particles;

    public Material material;
    private bool laserActive;
    LaserBeam beam;

    public GameObject player;
    public PlayerMovement movement;

    private bool levitateActive;
    private GameObject levitatedObject = null;
    private Rigidbody levitatedRigidbody = null;
    private Collider playerCollider;    // Reference to player's collider
    private Collider levitatedCollider; // Reference to levitated object's collider
    private Animator levitatedAnimator = null;

    private GameObject platformForPortalA = null; // Platform associated with Portal A
    private GameObject platformForPortalB = null; // Platform associated with Portal B

    public LayerMask portalPlacementLayer;

    [Header("Audio Settings")]
    public AudioClip portalPlacementSound; // Sound to play when a portal is placed
    private AudioSource audioSource;      // AudioSource to play sounds

    void Start()
    {
        particles.Stop();
        // Get the player's collider (assuming PortalGun is attached to the player)
        playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
        {
            // If the collider is on the parent object
            playerCollider = GetComponentInParent<Collider>();
        }

        // Initialize AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Right-click to shoot a portal
        if (Input.GetMouseButtonDown(1))
        {
            switch (InventorySelect.currentSelection.ID) {
                case 1:
                    ShootPortal();
                    break;

                case 2:
                    laserActive = true;
                    break;

                case 3:
                    levitateActive = true;
                    break;

                case 4:
                    if (SpawnLocation.coins >= 5) {
                        Slow();
                        SpawnLocation.coins -= 5;
                        coinCounterText.text = "x" + SpawnLocation.coins;
                        break;
                    }
                    else {
                        Debug.Log("Not enough coins");
                    }
                    break;
            }
        }

        if (Input.GetMouseButtonUp(1) && InventorySelect.currentSelection.ID == 2)
        {
            laserActive = false;
            Destroy(GameObject.Find("Laser Beam")); // Destroy the laser beam when button is released
        }

        if (laserActive) {
            ShootLaser();
        }

        if (levitateActive)
        {
            TryStartLevitation();

            if (Input.GetMouseButtonUp(1) && InventorySelect.currentSelection.ID == 3)
            {
                levitateActive = false;
                StopLevitation();
            }

            // Update levitated object position while holding
            if (levitatedObject != null)
            {
                UpdateLevitatedObjectPosition();
            }

            // Launch object with F while levitating
            if (Input.GetKeyDown(KeyCode.F) && levitatedObject != null)
            {
                levitateActive = false;
                LaunchObject();
            }
        }
    }

    void TryStartLevitation()
    {
        if (levitatedObject != null) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, levitationRange))
        {
            // Check for either Floating or RobotSphere tag
            if (hit.collider.CompareTag("Floating") || hit.collider.CompareTag("FinalRobot"))
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
        // Calculate desired position in front of the player using the appropriate distance
        float currentLevitationDistance = levitatedObject.CompareTag("RobotSphere") ? 
            robotSphereLevitationDistance : levitationDistance;
        
        Vector3 targetPosition = playerCamera.transform.position + 
                               playerCamera.transform.forward * currentLevitationDistance;

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

            // Only allow portals to be placed on valid surfaces
            if (!hit.collider.CompareTag("Portal") && !hit.collider.CompareTag("Enemy") &&
                !hit.collider.CompareTag("Non-Portal") && !hit.collider.CompareTag("Border"))
            {
                // Get the platform associated with the hit
                GameObject hitPlatform = GetPlatformFromHit(hit);

                // If both portals are active, destroy them before placing new ones
                if (portalA != null && portalB != null)
                {
                    Destroy(portalA);
                    Destroy(portalB);
                    portalA = null;
                    portalB = null;
                    platformForPortalA = null;
                    platformForPortalB = null;
                }

                // Place the first portal (Portal A) if none exist
                if (portalA == null)
                {
                    Vector3 portalPosition = hit.point + hit.normal * 0.05f;
                    portalA = Instantiate(portalPrefabA, portalPosition, Quaternion.LookRotation(hit.normal));
                    platformForPortalA = hitPlatform; // Associate Portal A with its platform
                    Debug.Log("Portal A placed at: " + hit.point);
                    
                    // Play portal placement sound
                    PlayPortalPlacementSound();
                }
                // Place the second portal (Portal B) if Portal A exists
                else if (portalB == null)
                {
                    Vector3 portalPosition = hit.point + hit.normal * 0.05f;
                    portalB = Instantiate(portalPrefabB, portalPosition, Quaternion.LookRotation(hit.normal));
                    platformForPortalB = hitPlatform; // Associate Portal B with its platform

                    // Link the two portals
                    portalA.GetComponent<Portal>().linkedPortal = portalB;
                    portalB.GetComponent<Portal>().linkedPortal = portalA;

                    Debug.Log("Portal B placed at: " + hit.point);
                    
                    // Play portal placement sound
                    PlayPortalPlacementSound();
                }
            }
        }
    }

    void PlayPortalPlacementSound()
    {
        if (audioSource != null && portalPlacementSound != null)
        {
            audioSource.PlayOneShot(portalPlacementSound, SettingsManager.soundVolume);
        }
    }

    void ShootLaser()
    {
        Destroy(GameObject.Find("Laser Beam"));
        beam = new LaserBeam(gameObject.transform.position, -gameObject.transform.forward, material);
    }


    void Slow() {
    if (!movement.slowPowerActive)
        {
            StartCoroutine(SlowEffect());
        }
    }

    IEnumerator SlowEffect()
{
    float slowDuration = 10f;
    float elapsedTime = 0f;

    Time.timeScale = 0.5f;
    movement.slowPowerActive = true;
    particles.Play();

    while (elapsedTime < slowDuration)
    {
        if (Time.deltaTime > 0)
        {
            elapsedTime += Time.unscaledDeltaTime;
        }
        yield return null;
    }

    Debug.Log("Slowed time ended");
    Time.timeScale = 1f;
    movement.slowPowerActive = false;
    particles.Stop();
}
    GameObject GetPlatformFromHit(RaycastHit hit)
    {
        // Check if the hit object or its parent has the "DisappearingPlatform" tag
        if (hit.collider.CompareTag("DisappearingPlatform"))
        {
            return hit.collider.gameObject;
        }
        else if (hit.collider.transform.parent != null &&
                 hit.collider.transform.parent.CompareTag("DisappearingPlatform"))
        {
            return hit.collider.transform.parent.gameObject;
        }
        return null; // Return null if not a disappearing platform
    }

    public void RemovePortalsOnPlatform(GameObject platform)
    {
        // Remove Portal A if it's on the disappearing platform
        if (platformForPortalA == platform)
        {
            Destroy(portalA);
            portalA = null;
            platformForPortalA = null;
            Debug.Log("Removed Portal A as the platform disappeared.");
        }

        // Remove Portal B if it's on the disappearing platform
        if (platformForPortalB == platform)
        {
            Destroy(portalB);
            portalB = null;
            platformForPortalB = null;
            Debug.Log("Removed Portal B as the platform disappeared.");
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

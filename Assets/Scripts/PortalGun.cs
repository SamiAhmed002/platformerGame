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
    public float levitationDistance = 3f;    // Distance at which object floats from player
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
    }

    void Update()
    {
        // Right-click to shoot a portal
        if (Input.GetMouseButtonDown(1) && !pauseGame.isPaused)
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
                    if (inventorySelect.coinCount >= 5) {
                        Slow();
                        inventorySelect.coinCount -= 5;
                        coinCounterText.text = "x" + inventorySelect.coinCount;
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

            // Launch object with F while levitating
            if (Input.GetKeyDown(KeyCode.F) && levitatedObject != null)
            {
                levitateActive = false;
                LaunchObject();
            }
        }

        // Update levitated object position while holding
        if (levitatedObject != null)
        {
            UpdateLevitatedObjectPosition();
        }
    }

    void ShootPortal()
    {
        // Raycast from the center of the screen (crosshair)
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Portal") == false && hit.collider.CompareTag("Enemy") == false && hit.collider.CompareTag("Border") == false)
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
                    // Offset the portal so it sticks to the wall
                    Vector3 portalPosition = hit.point + hit.normal * 0.05f; // Adjust the 0.05f based on portal thickness
                    portalA = Instantiate(portalPrefabA, portalPosition, Quaternion.LookRotation(hit.normal));
                    Debug.Log("Portal A placed at: " + hit.point);
                }
                // Place the second portal (Portal B) if Portal A exists
                else if (portalB == null)
                {
                    Vector3 portalPosition = hit.point + hit.normal * 0.05f; // Same adjustment
                    portalB = Instantiate(portalPrefabB, portalPosition, Quaternion.LookRotation(hit.normal));
                    Debug.Log("Portal B placed at: " + hit.point);

                    // Link the two portals
                    portalA.GetComponent<Portal>().linkedPortal = portalB;
                    portalB.GetComponent<Portal>().linkedPortal = portalA;
                }
            }
        }
    }

    void ShootLaser()
    {
        Destroy(GameObject.Find("Laser Beam"));
        beam = new LaserBeam(gameObject.transform.position, gameObject.transform.right, material);
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
}

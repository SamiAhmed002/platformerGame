using UnityEngine;

public class ButtonSpawnerController : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonObject;  // The button to press
    [SerializeField]
    private GameObject objectToSpawn;  // The prefab to spawn
    [SerializeField]
    private Transform spawnPoint;  // Where to spawn from (can be an empty GameObject at the ceiling)
    
    public float buttonOffset = 0.5f;  // How far the button moves down when pressed
    public float speed = 15f;          // Button movement speed
    public int maxSpawns = 5;  // Maximum number of objects that can be spawned
    
    private Vector3 buttonInitialPos;
    private bool canSpawn = true;      // To prevent rapid-fire spawning
    private float spawnCooldown = 1f;  // Time between spawns
    private float cooldownTimer = 0f;
    private int spawnCount = 0;  // Track how many objects we've spawned

    void Start()
    {
        if (buttonObject == null || objectToSpawn == null || spawnPoint == null)
        {
            Debug.LogError("Please assign all required fields in the inspector!");
            enabled = false;
            return;
        }

        buttonInitialPos = buttonObject.transform.position;
    }

    void Update()
    {
        if (IsButtonPressed())
        {
            // Move button down
            buttonObject.transform.position = Vector3.MoveTowards(
                buttonObject.transform.position,
                buttonInitialPos - new Vector3(0, buttonOffset, 0),
                speed * Time.deltaTime
            );

            // Spawn object if we can
            if (canSpawn)
            {
                SpawnObject();
                canSpawn = false;
                cooldownTimer = spawnCooldown;
            }
        }
        else
        {
            // Reset button position
            buttonObject.transform.position = Vector3.MoveTowards(
                buttonObject.transform.position,
                buttonInitialPos,
                speed * Time.deltaTime
            );
        }

        // Handle spawn cooldown
        if (!canSpawn)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canSpawn = true;
            }
        }
    }

    private bool IsButtonPressed()
    {
        Collider buttonCollider = buttonObject.GetComponent<Collider>();
        
        Collider[] overlappingColliders = Physics.OverlapBox(
            buttonCollider.bounds.center,
            buttonCollider.bounds.extents
        );

        foreach (Collider col in overlappingColliders)
        {
            if (col.attachedRigidbody != null || col.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnObject()
    {
        // Only spawn if we haven't reached the limit
        if (spawnCount < maxSpawns)
        {
            Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity);
            spawnCount++;
            
            // Optionally log when reaching the spawn limit
            if (spawnCount >= maxSpawns)
            {
                Debug.Log("Maximum spawn limit reached!");
            }
        }
    }
} 
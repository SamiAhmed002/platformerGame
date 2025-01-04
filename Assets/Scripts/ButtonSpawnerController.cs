using UnityEngine;
using System.Collections;

public class ButtonSpawnerController : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonObject;  // The button to press
    [SerializeField]
    private GameObject resetButtonObject;  // The reset button
    [SerializeField]
    private GameObject objectToSpawn;  // The prefab to spawn
    [SerializeField]
    private Transform spawnPoint;  // Where to spawn from
    
    public float buttonOffset = 0.5f;  // How far the button moves down when pressed
    public float speed = 15f;          // Button movement speed
    public int maxSpawns = 5;  // Maximum number of objects that can be spawned
    
    private Vector3 buttonInitialPos;
    private Vector3 resetButtonInitialPos;
    private bool isSpawning = false;
    private bool wasButtonPressed = false; // Track if button was already pressed
    private System.Collections.Generic.List<GameObject> spawnedObjects = new System.Collections.Generic.List<GameObject>(); // Using List instead of array

    void Start()
    {
        if (buttonObject == null || objectToSpawn == null || spawnPoint == null || resetButtonObject == null)
        {
            Debug.LogError("Please assign all required fields in the inspector!");
            enabled = false;
            return;
        }

        buttonInitialPos = buttonObject.transform.position;
        resetButtonInitialPos = resetButtonObject.transform.position;
    }

    void Update()
    {
        HandleMainButton();
        HandleResetButton();
    }

    private void HandleMainButton()
    {
        bool isPressed = IsButtonPressed(buttonObject);
        
        if (isPressed)
        {
            // Move button down
            buttonObject.transform.position = Vector3.MoveTowards(
                buttonObject.transform.position,
                buttonInitialPos - new Vector3(0, buttonOffset, 0),
                speed * Time.deltaTime
            );

            // Only start spawning if:
            // 1. Button wasn't pressed before
            // 2. We're not already spawning
            // 3. We haven't reached max objects
            if (!wasButtonPressed && !isSpawning && spawnedObjects.Count < maxSpawns)
            {
                StartCoroutine(SpawnSequence());
            }
            wasButtonPressed = true;
        }
        else
        {
            // Reset button position
            buttonObject.transform.position = Vector3.MoveTowards(
                buttonObject.transform.position,
                buttonInitialPos,
                speed * Time.deltaTime
            );
            wasButtonPressed = false;
        }
    }

    private void HandleResetButton()
    {
        if (IsButtonPressed(resetButtonObject))
        {
            // Move reset button down
            resetButtonObject.transform.position = Vector3.MoveTowards(
                resetButtonObject.transform.position,
                resetButtonInitialPos - new Vector3(0, buttonOffset, 0),
                speed * Time.deltaTime
            );

            // Destroy all spawned objects
            DestroySpawnedObjects();
        }
        else
        {
            // Reset button position
            resetButtonObject.transform.position = Vector3.MoveTowards(
                resetButtonObject.transform.position,
                resetButtonInitialPos,
                speed * Time.deltaTime
            );
        }
    }

    private bool IsButtonPressed(GameObject button)
    {
        Collider buttonCollider = button.GetComponent<Collider>();
        
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

    private IEnumerator SpawnSequence()
    {
        isSpawning = true;

        while (spawnedObjects.Count < maxSpawns)
        {
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity);
            spawnedObjects.Add(spawnedObject);
            
            if (spawnedObjects.Count >= maxSpawns)
            {
                Debug.Log("Maximum spawn limit reached!");
            }
            
            yield return new WaitForSeconds(1f);
        }

        isSpawning = false;
    }

    private void DestroySpawnedObjects()
    {
        // Stop any ongoing spawning
        StopAllCoroutines();
        isSpawning = false;

        // Destroy all spawned objects
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
    }
} 
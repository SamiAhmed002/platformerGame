using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    [Header("Attraction Settings")]
    public float attractionRange = 10f;        // Range within which objects are attracted
    public float maintainDistance = 3f;        // Distance to maintain from the attractor
    public float attractionSpeed = 5f;         // Speed at which objects move towards attractor

    [Header("Material Settings")]
    public Material defaultMaterial;           // Material when not attracting
    public Material attractingMaterial;        // Material when attracting objects

    private MeshRenderer meshRenderer;
    private bool isAttracting = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogWarning("No MeshRenderer found on this object!");
            return;
        }

        // Store the initial material if no default material is assigned
        if (defaultMaterial == null)
        {
            defaultMaterial = meshRenderer.sharedMaterial;
            Debug.Log($"Setting default material to: {defaultMaterial.name}");
        }

        // Ensure the mesh renderer starts with the default material
        meshRenderer.material = defaultMaterial;
        isAttracting = false;
    }

    private void FixedUpdate()
    {
        bool foundFinalRobot = false;

        // Find all colliders within the attraction range
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, attractionRange);

        foreach (Collider collider in nearbyColliders)
        {
            // Check if the object has the "FinalRobot" tag
            if (collider.CompareTag("FinalRobot"))
            {
                Debug.Log("Final robot detected by GravityAttractor.");
                foundFinalRobot = true;
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calculate direction and distance to the final robot
                    Vector3 direction = transform.position - collider.transform.position;
                    float distance = direction.magnitude;

                    // Only attract if the robot is further than maintainDistance
                    if (distance > maintainDistance + 0.1f) // Adding a small margin (0.1f)
                    {
                        Vector3 attractionForce = direction.normalized * attractionSpeed;
                        float strengthMultiplier = Mathf.Clamp01((distance - maintainDistance) / attractionRange);
                        attractionForce *= strengthMultiplier;
                        rb.velocity = attractionForce;
                    }
                    else
                    {
                        rb.velocity = Vector3.zero;

                        // Trigger the good ending when the robot is within maintainDistance
                        Debug.Log("Final robot reached maintainDistance. Triggering good ending.");
                        TriggerGoodEnding();
                    }
                }
            }
        }

        // Update material based on whether we're attracting objects
        UpdateMaterial(foundFinalRobot);
    }



    private void UpdateMaterial(bool isAttractingObjects)
    {
        if (meshRenderer != null && attractingMaterial != null)
        {
            if (isAttractingObjects && !isAttracting)
            {
                meshRenderer.material = attractingMaterial;
                isAttracting = true;
            }
            else if (!isAttractingObjects && isAttracting)
            {
                meshRenderer.material = defaultMaterial;
                isAttracting = false;
            }
        }
    }

    private void TriggerGoodEnding()
    {
        // Find the TriggerRobotManager and trigger the good ending
        Debug.Log("TriggerGoodEnding called in GravityAttractor.");
        TriggerRobotManager triggerManager = FindObjectOfType<TriggerRobotManager>();
        if (triggerManager != null)
        {
            triggerManager.TriggerGoodEnding(); // This will stop the countdown and load the good ending scene
        }
        else
        {
            Debug.LogWarning("TriggerRobotManager not found!");
        }
    }

}
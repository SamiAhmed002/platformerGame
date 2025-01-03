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
        bool foundFloatingObject = false;
        
        // Find all colliders within the attraction range
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, attractionRange);
        
        foreach (Collider collider in nearbyColliders)
        {
            // Check if the object has the "RobotSphere" tag
            if (collider.CompareTag("RobotSphere"))
            {
                foundFloatingObject = true;
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calculate direction and distance to the floating object
                    Vector3 direction = transform.position - collider.transform.position;
                    float distance = direction.magnitude;
                    
                    // Only attract if the object is further than maintainDistance
                    if (distance > maintainDistance)
                    {
                        // Normalize direction and scale by attraction speed
                        Vector3 attractionForce = direction.normalized * attractionSpeed;
                        
                        // The closer to maintainDistance, the weaker the attraction
                        float strengthMultiplier = (distance - maintainDistance) / attractionRange;
                        attractionForce *= strengthMultiplier;
                        
                        // Apply the force to the object
                        rb.velocity = attractionForce;
                    }
                    else
                    {
                        // If too close, gently push away
                        rb.velocity = Vector3.zero;
                    }
                }
            }
        }
        
        // Update material based on whether we're attracting objects
        UpdateMaterial(foundFloatingObject);
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

    // Optional: Visualize the attraction range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maintainDistance);
    }
} 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    public GameObject portalPrefabA;  // Prefab for Portal A
    public GameObject portalPrefabB;  // Prefab for Portal B
    public Camera playerCamera;       // Reference to player's camera
    public float maxDistance = 500f;  // Maximum range of portal shooting

    private GameObject portalA = null;  // Instance of Portal A
    private GameObject portalB = null;  // Instance of Portal B

    void Update()
    {
        // Right-click to shoot a portal
        if (Input.GetMouseButtonDown(1))
        {
            ShootPortal();
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
            if (hit.collider.CompareTag("Portal") == false)
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
}
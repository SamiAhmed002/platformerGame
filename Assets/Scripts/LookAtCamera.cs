using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        // Ensure the Canvas rotates to face the camera
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
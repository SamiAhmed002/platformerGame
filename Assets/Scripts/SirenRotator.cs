using UnityEngine;

public class SirenRotator : MonoBehaviour
{
    public float rotationSpeed = 100f; // Adjust speed as needed

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
using UnityEngine;

public class CollectibleHover : MonoBehaviour
{
    public float height = 0.2f;
    public float speed = 2f;

    public float rotation = 50f; 

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Hover
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotation
        transform.Rotate(Vector3.up * rotation * Time.deltaTime, Space.World);
    }
}

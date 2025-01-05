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
        // Make coin hover up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Gradually rotate coin
        transform.Rotate(Vector3.up * rotation * Time.deltaTime, Space.World);
    }
}

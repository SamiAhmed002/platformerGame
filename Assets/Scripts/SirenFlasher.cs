using UnityEngine;

public class SirenFlasher : MonoBehaviour
{
    private Light sirenLight;
    private float timer = 0f;
    public float flashSpeed = 2f; // Speed of flashing
    public float maxIntensity = 100f; // Maximum intensity value that can be set in Inspector

    void Start()
    {
        sirenLight = GetComponent<Light>();
    }

    void Update()
    {
        timer += Time.deltaTime * flashSpeed;
        sirenLight.intensity = Mathf.PingPong(timer, maxIntensity) * 3; // Using maxIntensity instead of hardcoded value
    }
}
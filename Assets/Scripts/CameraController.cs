using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public Transform target;
    public float sensitivityX;
    public float sensitivityY;
    private float pitch = 0f;
    private float mouseX;
    private float mouseY;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        transform.position = target.position;
        transform.RotateAround(target.position, Vector3.up, mouseX);

        transform.LookAt(target);
        transform.eulerAngles = new Vector3(pitch, transform.eulerAngles.y, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponent : MonoBehaviour
{
    public float Sensitivity = 2f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        yAxis = Input.GetAxis("Mouse X");
        xAxis = Input.GetAxis("Mouse Y");
        transform.eulerAngles -= new Vector3(xAxis, yAxis * -1, 0) * Sensitivity;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Vector3 move = transform.right * x + transform.forward * z;

        float speedMultiplier = 10f;

        if (Input.GetKey(KeyCode.LeftShift))
            speedMultiplier *= 4f;

        if (Input.GetKey(KeyCode.LeftControl))
            speedMultiplier *= 0.1f;

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += speedMultiplier * Time.deltaTime * transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= speedMultiplier * Time.deltaTime * transform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= speedMultiplier * Time.deltaTime * transform.right;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += speedMultiplier * Time.deltaTime * transform.right;
        }
    }

    private float xAxis = 0f;
    private float yAxis = 0f;
}

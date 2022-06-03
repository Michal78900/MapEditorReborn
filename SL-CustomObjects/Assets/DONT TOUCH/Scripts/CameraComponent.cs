using UnityEngine;

public class CameraComponent : MonoBehaviour
{
    public float TurnSpeed = 5.0f;
    public float MoveSpeed = 4.0f;

    private void Start() => Cursor.lockState = CursorLockMode.Locked;

    private void Update()
    {
        float multplier = 1f;

        if (Input.GetKey(KeyCode.LeftShift))
            multplier = 10f;

        MouseAiming();
        KeyboardMovement(multplier);
    }

    private void MouseAiming()
    {
        // get the mouse inputs
        float y = Input.GetAxis("Mouse X") * TurnSpeed;
        rotX += Input.GetAxis("Mouse Y") * TurnSpeed;

        // clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, -90f, 90f);

        // rotate the camera
        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
    }

    private void KeyboardMovement(float multiplier = 1f)
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * MoveSpeed * multiplier * Time.deltaTime);
    }

    private float rotX;
}

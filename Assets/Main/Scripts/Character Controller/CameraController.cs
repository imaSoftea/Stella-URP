using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 150f;
    public float distanceFromPlayer = 2f; 
    public float heightAdjust = 1.0f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89.9f, 89.9f);

        Quaternion cameraRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.position = playerBody.position - (cameraRotation * Vector3.forward * distanceFromPlayer);
        transform.LookAt(playerBody.position); 
        transform.position += new Vector3(0, heightAdjust, 0);
    }
}

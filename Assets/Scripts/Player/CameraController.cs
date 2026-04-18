using UnityEngine;

// Handles first-person camera rotation based on mouse input
public class CameraController : MonoBehaviour
{
    public float sensitivity = 200f;

    internal Camera cam;
    private float xRotation = 0f;

    void Start()
    {
        // Lock and hide cursor for first-person camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Read mouse input scaled by sensitivity
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Apply vertical rotation (clamped to prevent over-rotation)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply horizontal rotation to the player body
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}

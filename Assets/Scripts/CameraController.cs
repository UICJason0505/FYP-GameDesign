using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f;      // Increased speed for camera movement (WASD)
    public float rotateSpeed = 70f;  // Increased speed for camera rotation (Q/E)
    public float zoomSpeed = 500f;   // Increased speed for zooming with the mouse scroll

    public float minY = 10f;
    public float maxY = 60f;

    private Vector3 lastMousePosition;

    void Update()
    {
        HandleMovement();    // Handle WASD movement
        HandleRotate();      // Handle rotation with Q/E keys
        HandleZoom();        // Handle zooming with mouse scroll
    }
    // Handle camera movement using WASD (global space)
    void HandleMovement()
    {
        // Create movement vector (world-space directions)
        Vector3 moveDirection = Vector3.zero;

        // Check for WASD input for movement
        if (Input.GetKey(KeyCode.W))
            moveDirection += Vector3.forward; // Move forward along world-space Z-axis

        if (Input.GetKey(KeyCode.S))
            moveDirection += Vector3.back;   // Move backward along world-space Z-axis

        if (Input.GetKey(KeyCode.A))
            moveDirection += Vector3.left;   // Move left along world-space X-axis

        if (Input.GetKey(KeyCode.D))
            moveDirection += Vector3.right;  // Move right along world-space X-axis

        // Apply movement with panSpeed
        transform.position += moveDirection * panSpeed * Time.deltaTime;
    }

    // Handle camera rotation using Q/E keys
    void HandleRotate()
    {
        // Rotate left (Q) and right (E) around the Y-axis
        if (Input.GetKey(KeyCode.Q))
            transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.E))
            transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }

    // Handle zoom using the mouse scroll wheel
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 direction = transform.forward;

        // Only move in the forward direction (zooming in/out)
        Vector3 newPosition = transform.position + direction * scroll * Time.deltaTime * zoomSpeed;

        // Limit Y position to stay within minY and maxY
        if (newPosition.y > minY && newPosition.y < maxY)
            transform.position = newPosition;
    }
}

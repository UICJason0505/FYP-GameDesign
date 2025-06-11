using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float rotateSpeed = 70f;
    public float zoomSpeed = 500f;

    public float minY = 10f;
    public float maxY = 60f;

    private Vector3 lastMousePosition;

    void Update()
    {
        HandlePan();
        HandleRotate();
        HandleZoom();
    }

        void HandlePan()
    {
        if (Input.GetMouseButtonDown(1))
            lastMousePosition = Input.mousePosition;

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;

            // 将屏幕位移转换为世界位移
            Vector3 right = transform.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up); // 保证在水平面
            Vector3 move = (right * -delta.x + forward * -delta.y) * panSpeed * Time.deltaTime;

            transform.position += move;
            lastMousePosition = Input.mousePosition;
        }
    }


    void HandleRotate()
    {
        if (Input.GetKey(KeyCode.Q))
            transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.E))
            transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 direction = transform.forward;
        Vector3 newPosition = transform.position + direction * scroll * Time.deltaTime * zoomSpeed;

        // 限制 Y 高度
        if (newPosition.y > minY && newPosition.y < maxY)
            transform.position = newPosition;
    }
}

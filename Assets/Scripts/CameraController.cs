using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f;
    public float rotateSpeed = 70f;
    public float zoomSpeed = 0.5f;    // 鼠标滚轮灵敏度（可按需调）
    public float smoothTime = 0.12f;  // 平滑时间（越小越快）

    public float minY = 3;
    public float maxY = 20;

    private float targetY;
    private float zoomVelocity = 0f;

    void Start()
    {
        // 初始化目标高度为当前高度，避免跳变
        targetY = transform.position.y;
    }

    void Update()
    {
        HandleMovement();
        HandleRotate();
        HandleZoomSmooth();
    }

    void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        Vector3 fw = transform.forward;
        fw.y = 0;
        fw.Normalize();

        Vector3 rg = transform.right;
        rg.y = 0;
        rg.Normalize();

        if (Input.GetKey(KeyCode.W)) moveDirection += fw;
        if (Input.GetKey(KeyCode.S)) moveDirection -= fw;
        if (Input.GetKey(KeyCode.A)) moveDirection -= rg;
        if (Input.GetKey(KeyCode.D)) moveDirection += rg;
        // 按住 Shift 时移动速度 *2
        float finalSpeed = panSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            finalSpeed *= 2f;
        transform.position += moveDirection * panSpeed * Time.deltaTime;
    }

    void HandleRotate()
    {
        if (Input.GetKey(KeyCode.Q))
            transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.E))
            transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }

    // 平滑升降（只改变 Y）
    void HandleZoomSmooth()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            // 滚轮向前/向上通常返回正值 —— 根据你想要的方向调整符号
            // 这里用减号：滚轮向前（zoom in）会降低摄像机高度（靠近地面）
            targetY -= scroll * zoomSpeed;
            targetY = Mathf.Clamp(targetY, minY, maxY);
        }

        // 平滑过渡到目标高度
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref zoomVelocity, smoothTime);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

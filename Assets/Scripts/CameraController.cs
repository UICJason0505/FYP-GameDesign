using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f;
    public float rotateSpeed = 70f;

    [Header("Two-Level Zoom")]
    public float lowViewY = 5f;      // 低视角
    public float highViewY = 50f;    // 高空俯视
    public float zoomSmoothTime = 0.2f;

    private float targetCameraY;
    private float zoomVelocityY;

    public Transform followTarget;     // 当前需要对准的棋子
    public float followSmooth = 0.2f;  // 平滑跟随速度

    [Header("Center Offset")]
    public float centerOffset = 3f;    // 让棋子真正处于屏幕中央的偏移量

    void Start()
    {
        // 初始化为当前高度
        targetCameraY = transform.position.y;
    }

    void Update()
    {
        HandleMovement();
        HandleRotate();
        HandleZoomTwoLevel();
        HandleFollowTarget();
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

        float finalSpeed = panSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            finalSpeed *= 2f;

        transform.position += moveDirection * finalSpeed * Time.deltaTime;
    }

    void HandleRotate()
    {
        if (Input.GetKey(KeyCode.Q))
            transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.E))
            transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }

    // 二段式变焦（只变 Y）
    void HandleZoomTwoLevel()
    {
        // 中键点击切换两个高度
        if (Input.GetMouseButtonDown(2))   // 2 = Middle Mouse Button
        {
            // 判断当前更接近哪个档位
            if (Mathf.Abs(targetCameraY - lowViewY) < 0.1f)
                targetCameraY = highViewY;    // 当前是低 → 切换高
            else
                targetCameraY = lowViewY;     // 当前是高 → 切换低
        }

        // 平滑过渡到目标高度
        float newY = Mathf.SmoothDamp(
            transform.position.y,
            targetCameraY,
            ref zoomVelocityY,
            zoomSmoothTime
        );

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }


    void HandleFollowTarget()
    {
        if (followTarget == null) return;

        Vector3 forwardFlat = transform.forward;
        forwardFlat.y = 0;
        forwardFlat.Normalize();

        Vector3 offset = -forwardFlat * centerOffset;

        Vector3 currentPos = transform.position;

        Vector3 targetPos = new Vector3(
            followTarget.position.x + offset.x,
            currentPos.y,
            followTarget.position.z + offset.z
        );

        transform.position = Vector3.Lerp(
            currentPos,
            targetPos,
            Time.deltaTime / followSmooth
        );
    }
}

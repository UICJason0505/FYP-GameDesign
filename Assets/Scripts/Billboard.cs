using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    public Transform visualRoot;
    public float yOffset = 180; // 新增偏移角度

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (visualRoot == null || cam == null) return;

        Vector3 dir = cam.transform.position - visualRoot.position;
        dir.y = 0;
        visualRoot.forward = -dir;

        // 应用偏移角度
        visualRoot.Rotate(0, yOffset, 0);
    }
}

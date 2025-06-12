using UnityEngine;

/// <summary>
/// 连续沿 X 轴正方向移动的脚本
/// Attach this to any GameObject to make it move steadily along +X.
/// </summary>
public class MoveTest : MonoBehaviour
{
    [Tooltip("Units per second（每秒移动的世界单位）")]
    public float speed = 2f;

    void Update()
    {
        // Time.deltaTime 让移动与帧率无关
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }
}
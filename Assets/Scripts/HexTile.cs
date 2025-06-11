using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Vector3 GetTopCenter()
    {
        // 中心点 + 格子高度一半（假设为 0.5f）
        return transform.position + Vector3.up * 0.5f;
    }

    // 检查是否已有棋子（可拓展为占用标志）
    public bool IsOccupied = false;
}

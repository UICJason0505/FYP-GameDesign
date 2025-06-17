using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacebleObject : MonoBehaviour
{
    
    public bool Placed { get; private set; }  // 判断能否被放置
    public Vector3Int Size { get; private set; } = new Vector3Int(2, 2, 1);  // 固定大小，宽度为 2，高度为 sqrt(3) 大约为 1.732，取为 2（包围盒的宽度），高度为 1
    private Vector3[] Vertices;

    public BoundsInt area; // 占用区域

    // 从碰撞箱中心获取尖顶向六边形的六个顶点
    private void GetColliderVertexPositionsLocal()
    {
        BoxCollider b = gameObject.GetComponent<BoxCollider>();
        Vertices = new Vector3[6];
        Vertices[0] = b.center + new Vector3(1.0f, 0, 0);
        Vertices[1] = b.center + new Vector3(0.5f, 0, Mathf.Sqrt(3) / 2.0f);
        Vertices[2] = b.center + new Vector3(-0.5f, 0, Mathf.Sqrt(3) / 2.0f);
        Vertices[3] = b.center + new Vector3(- 1.0f, 0, 0);
        Vertices[4] = b.center + new Vector3(-0.5f, 0, -Mathf.Sqrt(3) / 2.0f);
        Vertices[5] = b.center + new Vector3(0.5f, 0, -Mathf.Sqrt(3) / 2.0f);
    }

    // 获得transform初始位置
    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }

    // 初始化物体的区域（坐标 + 占用范围）
    private void InitialiseArea()
    {
        area = new BoundsInt(new Vector3Int(0, 0, 0), Size);
    }

    // 放置逻辑
    public virtual void Place()
    {
        ObjectDrag drag = gameObject.GetComponent<ObjectDrag>();
        Destroy(drag);

        Placed = true;
    }


    // Process
    private void Awake()
    {
        GetColliderVertexPositionsLocal();
    }

    private void Start()
    {
        InitialiseArea();
    }

}

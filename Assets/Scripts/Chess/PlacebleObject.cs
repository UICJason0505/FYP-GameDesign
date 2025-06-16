using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacebleObject : MonoBehaviour
{

    public bool Placed { get; private set; }
    public Vector3Int Size { get; private set; }
    private Vector3[] Vertices;

    public BoundsInt area;

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

    private void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[Vertices.Length];

        for (int i = 0; i < Vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(Vertices[i]);
            vertices[i] = BuildingSystem.instance.gridLayout.WorldToCell(worldPos);
        }

        Size = new Vector3Int(Mathf.Abs((vertices[0] - vertices[1]).x) + 1,
                              Mathf.Abs((vertices[0] - vertices[3]).y) + 1,
                              1); // needed to be 1

        Debug.Log("Building Size (x,y,z):" + Size.ToString());
    }

    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }

    private void Awake()
    {
        GetColliderVertexPositionsLocal();
    }


    private void Start()
    {
        CalculateSizeInCells();
        InitialiseArea();
    }

    private void InitialiseArea()
    {
        area = new BoundsInt(new Vector3Int(0, 0, 0), Size);
    }

    public virtual void Place()
    {
        ObjectDrag drag = gameObject.GetComponent<ObjectDrag>();
        Destroy(drag);

        Placed = true;

        // Invoke events of placement here

    }

}

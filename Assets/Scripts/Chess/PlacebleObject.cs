using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HexMath;

public class PlacebleObject : MonoBehaviour
{
    private Collider[] allCols;
    private Vector3 mouseOffset;
    public bool Placed { get; private set; }
    public bool Dragging { get; private set; }
    public Coordinates GridPos { get; private set; }
    private void Awake()
    {
        if (this.CompareTag("King"))
        {
            Placed = true;
            return;
        }
        allCols = GetComponentsInChildren<Collider>(true);
        foreach (var c in allCols) c.enabled = false;
    }
    private void OnMouseDown()
    {
        if (Placed) return;
        mouseOffset = transform.position - GridBuildingSystem.GetMousePos();
    }
    private void Update()
    {
        if (Placed) return;
        Vector3 root = GridBuildingSystem.GetMousePos() + mouseOffset;
        transform.position = GridBuildingSystem.Instance.Snap(root);
    }
    public void BeginDrag()
    {
        Placed = false;
        foreach (var c in allCols) c.enabled = false;
        mouseOffset = transform.position - GridBuildingSystem.GetMousePos();
    }
    public void FinalizePlacement(Coordinates a, Vector3 center)
    {
        Placed = true;
        GridPos = a;
        transform.position = center + Vector3.up * 0.7f;
        foreach (var c in allCols) c.enabled = true;
    }

}

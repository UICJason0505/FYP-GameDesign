using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PathPreviewManager : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<Vector3> pathPoints = new List<Vector3>();

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.yellow;
    }

    public void ShowPath(List<Vector3> points)
    {
        pathPoints = points;
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());
    }

    public void ClearPath()
    {
        pathPoints.Clear();
        lineRenderer.positionCount = 0;
    }
}

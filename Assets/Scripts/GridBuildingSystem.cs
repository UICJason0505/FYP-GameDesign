using System.Collections;
using System.Collections.Generic;
using Autodesk.Fbx;
using UnityEngine;
using UnityEngine.Tilemaps;
using static HexMath;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }
    [Header("Prefabs")]
    public GameObject Chess1, Chess2;
    private PlacebleObject current;
    private readonly HashSet<Coordinates> occupied = new(); 
    private void Awake() => Instance = this;
    private int MeeleCount = 0;
    private int RangedCount = 0;
    public static Vector3 GetMousePos(float offset = 0f)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
            return hit.point + Vector3.up * offset;
        return Vector3.zero;

    }
    private bool TryGetHoveredTile(out HexTile tile)
    {
        tile = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit))
        {
            return false;
        }
        tile = hit.collider.GetComponent<HexTile>();
        Debug.Log($"Hit tile  Coordinates = ({tile.coordinates.x}, {tile.coordinates.z})");
        return tile != null;
    }
    public Vector3 Snap(Vector3 rootPos)
    {
        if (TryGetHoveredTile(out HexTile tile))
            return tile.centerWorld + Vector3.up * 0.5f;
        rootPos.y = 0.5f;                     
        return rootPos;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) StartPlacing(Chess1);
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartPlacing(Chess2);
        if (current == null) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetHoveredTile(out HexTile tile) && !occupied.Contains(tile.coordinates))
            {
                current.FinalizePlacement(tile.coordinates, tile.centerWorld);
                occupied.Add(tile.coordinates);
                current = null;
            }
            else Debug.LogWarning("∑«∑®Œª÷√£°");
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(current.gameObject);
            current = null;
        }
    }
    private void StartPlacing(GameObject prefab)
    {
        if(current) Destroy(current.gameObject);
        current = Instantiate(prefab, Vector3.up * 2, Quaternion.identity).GetComponent<PlacebleObject>();
        Chess chess = current.GetComponent<Chess>();
        if (prefab == Chess1)
            chess.Init("Melee", ++MeeleCount, null);
        else
            chess.Init("Ranged", ++RangedCount, null);
    }
}

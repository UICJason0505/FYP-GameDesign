using UnityEngine;
using System.Collections.Generic;

public class UnitSelectionManager : MonoBehaviour
{
    public PathPreviewManager pathPreviewManager;

    private UnitController selectedUnit;
    private HexTile selectedTile;
    private HexTile hoveredTile;

    void Update()
    {
        // 点击检测
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                UnitController unit = hit.collider.GetComponent<UnitController>();
                if (unit != null)
                {
                    if (selectedUnit != null) selectedUnit.Deselect();
                    selectedUnit = unit;
                    unit.Select();
                    selectedTile = GetTileUnder(unit.transform.position);
                    pathPreviewManager.ClearPath();
                }
                else
                {
                    HexTile tile = hit.collider.GetComponent<HexTile>();
                    if (selectedUnit != null && tile != null && !tile.IsOccupied)
                    {
                        // 点击目标格子，开始移动
                        MoveUnitAlongPath(selectedUnit, selectedTile, tile);
                        pathPreviewManager.ClearPath();
                        selectedUnit.Deselect();
                        selectedUnit = null;
                    }
                }
            }
        }

        // 鼠标悬停路径预览
        if (selectedUnit != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                HexTile tile = hit.collider.GetComponent<HexTile>();
                if (tile != null && tile != hoveredTile && !tile.IsOccupied)
                {
                    hoveredTile = tile;
                    List<Vector3> path = ComputeHexLine(selectedTile, hoveredTile);
                    pathPreviewManager.ShowPath(path);
                }
            }
        }
    }

    // 简化版：直接首尾连线（你可用A*或六边形路径算法替换）
    List<Vector3> ComputeHexLine(HexTile start, HexTile end)
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(start.GetTopCenter());
        path.Add(end.GetTopCenter());
        return path;
    }

    HexTile GetTileUnder(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<HexTile>();
        }
        return null;
    }

    void MoveUnitAlongPath(UnitController unit, HexTile start, HexTile end)
    {
        // TODO：我们下一步写移动动画
        unit.transform.position = end.GetTopCenter() + Vector3.up * 0.5f;
        start.IsOccupied = false;
        end.IsOccupied = true;
    }
}

using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unitPrefab; // 拖入你的棋子 prefab
    private HexTile pendingTile = null;

    void Update()
    {
        // 左键点击检测
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                HexTile tile = hit.collider.GetComponent<HexTile>();

                if (tile != null && !tile.IsOccupied)
                {
                    pendingTile = tile;
                    Debug.Log("点击格子：按 N 生成棋子");
                }
                else
                {
                    pendingTile = null; // 点击其他内容，取消提示
                }
            }
        }

        // 按 N 生成棋子
        if (pendingTile != null && Input.GetKeyDown(KeyCode.N))
        {
            Vector3 spawnPos = pendingTile.GetTopCenter();
            GameObject unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
            pendingTile.IsOccupied = true;

            Debug.Log("棋子已生成");
            pendingTile = null;
        }
    }
}

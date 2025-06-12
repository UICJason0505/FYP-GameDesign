using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject unitPrefab;
    private HexTile pendingTile = null;

    void Update()
    {
        // 鼠标点击空格子
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                HexTile tile = hit.collider.GetComponent<HexTile>();

                if (tile != null && !tile.IsOccupied)
                {
                    pendingTile = tile;
                    Debug.Log("点击了空格子，按 N 生成棋子");
                }
                else
                {
                    pendingTile = null;
                }
            }
        }

        // 按 N 键生成棋子
        if (pendingTile != null && Input.GetKeyDown(KeyCode.N))
        {
            Vector3 spawnPos = pendingTile.transform.position + Vector3.up * 0.5f;

            GameObject unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
            pendingTile.IsOccupied = true;

            Debug.Log("棋子已生成于：" + spawnPos);
            pendingTile = null;
        }
    }
}

using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexTilePrefab;
    public int width = 5;
    public int height = 5;
    public float hexRadius = 1f;
    public float hexHeight = 1f;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        float xOffset = hexRadius * 1.5f;
        float zOffset = Mathf.Sqrt(3) * hexRadius;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float xPos = x * xOffset;
                float zPos = z * zOffset + (x % 2 == 0 ? 0 : zOffset / 2);

                GameObject hex = Instantiate(hexTilePrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
                hex.transform.localScale = new Vector3(1, hexHeight, 1);
            }
        }
    }
}

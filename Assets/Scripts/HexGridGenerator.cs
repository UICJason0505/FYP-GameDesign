using UnityEngine;
using System.Collections;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexTilePrefab;
    public int width = 5;
    public int height = 5;
    public float hexRadius = 0.5f;
    public float hexHeight = 0.2f;

    public float normalY = 0.2f;
    public float sunkenOffset = -0.7f; // 沉下去的相对值
    public float transitionTime = 1.0f;

    private bool isSunken = false;

    void Start()
    {
        GenerateGrid();
        normalY = transform.position.y; // 记录原始浮起位置
    }

        public void Sink()
    {
        if (!isSunken) StartCoroutine(MoveGridY(normalY + sunkenOffset));
        isSunken = true;
    }

    public void Rise()
    {
        if (isSunken) StartCoroutine(MoveGridY(normalY));
        isSunken = false;
    }

    IEnumerator MoveGridY(float targetY)
    {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(start.x, targetY, start.z);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / transitionTime;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        transform.position = end;
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
                Vector3 spawnPos = transform.position + new Vector3(xPos, 0, zPos);

                GameObject hex = Instantiate(hexTilePrefab, spawnPos, Quaternion.identity, transform);
                hex.transform.localScale = new Vector3(1, hexHeight, 1);
            }
        }
    }
}

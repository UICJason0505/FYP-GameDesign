using UnityEngine;
using static HexMath;
public class HexTile : MonoBehaviour
{
    public Coordinates coordinates;            
    public Vector3 centerWorld;
    public Renderer rend;
    public bool canAttack = true;
    public Color highlighColor = Color.gray;
    public Color baseColor;
    [Header("格子数值（棋子经过时加成）")]
    public int tileValue = 1;
    private void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        centerWorld = mr.bounds.center;
        float a = Mathf.Max(mr.bounds.size.x, mr.bounds.size.z) * 0.5f + 0.1f;
        coordinates = HexMath.WorldToCoordinates(centerWorld, a);
        TileManager.Register(this, coordinates);
        rend = GetComponent<Renderer>();
        baseColor = rend.material.color;
    }

    public int GetTileValue()
    {
        return tileValue;
    }

    public void SetTileValue(int newValue)
    {
        tileValue = newValue;
    }
    public void HighlightTile()
    {
        rend.material.color = highlighColor;
        canAttack = true;
    }
    public void ResetTile()
    {
        rend.material.color = baseColor;
        canAttack = false;
    }
}

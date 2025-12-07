using UnityEngine;
using static HexMath;
public class HexTile : MonoBehaviour
{   
    public static float radius; // 半径
    public Coordinates coordinates;            
    public Vector3 centerWorld;
    public Renderer rend;
    public bool canAttack = true;
    public Color highlighColor = Color.gray;
    public Color baseColor;
    public bool attackable = false;
    public static Chess[] allChess;
    [Header("格子数值（棋子经过时加成）")]
    public int tileValue = 1;
    private void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        centerWorld = mr.bounds.center;
        radius = Mathf.Max(mr.bounds.size.x, mr.bounds.size.z) * 0.5f;
        coordinates = HexMath.WorldToCoordinates(centerWorld, radius);
        TileManager.Register(this, coordinates);
        rend = GetComponent<Renderer>();
        baseColor = rend.material.color;
    }
    private void Start()
    {
        HexTile.RefreshAllChess();
    }
    public static void RefreshAllChess()
    {
        allChess = FindObjectsOfType<Chess>();
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
        
        attackable = true;
        if(attackable && canAttack == true) attackable = true;
        else attackable = false;
        if(attackable == true)rend.material.color = highlighColor;
    }
    public void ResetTile()
    {
        rend.material.color = baseColor;
        attackable = false;
    }

    public static Chess GetChessAt(HexMath.Coordinates c)
    {
        foreach (var unit in allChess)
        {
            if (unit == null) continue;

            if (unit.position.x == c.x && unit.position.z == c.z)
                return unit;
        }
        return null;
    }
}

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
    public static Chess[] allChess; // 场上所有棋子数组
    [Header("格子数值（棋子经过时加成）")]
    public int tileValue = 1;
    [Header("占用状态（碰撞检测驱动）")]
    public bool isOccupied = false;
    public Chess occupyingChess = null;

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

    private void Update()
    {
        if (occupyingChess != null)
        {
            float dist = Vector3.Distance(occupyingChess.transform.position, centerWorld);
            if (dist > radius * 1.2f)
            {
                // 离太远视为离开
                isOccupied = false;
                occupyingChess = null;
            }
        }
        if (this.gameObject.layer == LayerMask.NameToLayer("Water")) isOccupied = true;
    }

    public static void RefreshAllChess() // 刷新场上所有棋子信息，存入数组中
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
    private void OnTriggerEnter(Collider other)
    {
        Chess chess = other.GetComponentInParent<Chess>();
        if (chess == null) return;

        // 标记占用
        isOccupied = true;
        occupyingChess = chess;
    }

    private void OnTriggerExit(Collider other)
    {
        Chess chess = other.GetComponentInParent<Chess>();
        if (chess == null) return;

        // 只有同一个棋子离开时才清空
        if (occupyingChess == chess)
        {
            isOccupied = false;
            occupyingChess = null;
        }
    }

}

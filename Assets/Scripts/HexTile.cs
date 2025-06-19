using UnityEngine;
using static HexMath;
public class HexTile : MonoBehaviour
{
    public Coordinates coordinates;            
    public Vector3 centerWorld;    

    private void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        centerWorld = mr.bounds.center;
        float a = Mathf.Max(mr.bounds.size.x, mr.bounds.size.z) * 0.5f + 0.1f;
        coordinates = HexMath.WorldToCoordinates(centerWorld, a);
        TileManager.Register(this, coordinates);
    }
}

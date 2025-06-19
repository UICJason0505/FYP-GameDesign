using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static HexMath;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }
    private static readonly Dictionary<Coordinates, HexTile> dict = new();

    public static void Register(HexTile tile, Coordinates a)
    {
        if (!dict.ContainsKey(a))
            dict.Add(a, tile);
    }

    public bool TryGetTile(Coordinates a, out HexTile tile) => dict.TryGetValue(a, out tile);

    public bool TileExists(Coordinates a) => dict.ContainsKey(a);

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
}

using UnityEngine;

public class HexMath : MonoBehaviour
{
    public static Vector3 originWorld = Vector3.zero + Vector3.up * 0.8f;
    [System.Serializable]
    public struct Coordinates
    {
        public int x;
        public int z;
        public int y => -x - z;
        public Coordinates(int x, int z) { this.x = x; this.z = z; }
        public override int GetHashCode() => (x, z).GetHashCode();
    }

    private static Coordinates RoundAxial(float qf, float rf)
    {
        float sf = -qf - rf;
        int qi = Mathf.RoundToInt(qf);
        int ri = Mathf.RoundToInt(rf);
        int si = Mathf.RoundToInt(sf);

        float dq = Mathf.Abs(qi - qf);
        float dr = Mathf.Abs(ri - rf);
        float ds = Mathf.Abs(si - sf);

        if (dq > dr && dq > ds) qi = -ri - si;
        else if (dr > ds) ri = -qi - si;
        else si = -qi - ri;

        return new Coordinates(qi, ri);
    }

    public static Coordinates WorldToCoordinates(Vector3 world, float radius)
    {
        Vector3 rel = world - originWorld;
        float qf = (2f / 3f * rel.z) / radius;
        float rf = (-1f / 3f * rel.z + Mathf.Sqrt(3f) / 3f * rel.x) / radius;
        return RoundAxial(qf, rf);
    }

    public static Vector3 CoordinatesToWorld(Coordinates c, float radius, float y = 0f)
    {
        float wx = radius * Mathf.Sqrt(3f) * (c.z + c.x * 0.5f);
        float wz = radius * 1.5f * c.x;
        return originWorld + new Vector3(wx, y, wz);
    }

    // 计算两个六边形坐标之间的格子距离（立方/三轴坐标距离）
    public static int HexDistance(Coordinates a, Coordinates b)
    {
        int dx = a.x - b.x;
        int dz = a.z - b.z;
        int dy = a.y - b.y; // 由属性自动计算

        return (Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz)) / 2;
    }
}

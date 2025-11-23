using System.Linq;
using UnityEngine;
using static HexMath;
using static MovingObject;
public class Knight : MonoBehaviour
{
    [Header("基础属性")]
    public int number = 1;
    public Coordinates position;
    public int attackArea = 1;
    public bool isInAttackMode = false;

    [Header("系统引用")]
    public UnitInfoPanelController panel;
    public Renderer rend;

    [Header("技能参数")]
    private int dashCooldown = 0;
    private const int dashCDMax = 3;
    private bool isDashing = false;

    void Start()
    {
        if (panel == null)
            panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault();
    }

    void Update()
    {
        if (SelectionManager.selectedObj != gameObject) return;

        if (panel != null && Input.GetMouseButtonDown(1))
            panel.Hide();

        if (Input.GetKeyDown(KeyCode.X))
        {
            isInAttackMode = !isInAttackMode;
        }

        if (isInAttackMode && Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        if (Input.GetKeyDown(KeyCode.C) && dashCooldown == 0)
        {
            ActivateDash();
        }

        if (dashCooldown > 0)
            dashCooldown--;
    }

    void OnMouseDown()
    {
        SelectionManager.selectedObj = gameObject;
        if (panel != null)
            panel.ShowUnit("Knight", number);
    }

    // 攻击逻辑（含盾卫嘲讽响应）
    void TryAttack()
    {
        Vector3 mouseWorldPos = GridBuildingSystem.GetMousePos();
        Coordinates targetCoord = HexMath.WorldToCoordinates(mouseWorldPos, 1f);

        // 若场上存在激活嘲讽的盾卫 → 强制攻击该盾卫
        var tauntingShield = FindObjectsOfType<ShieldGuard>().FirstOrDefault(s => s.isTauntActive);
        if (tauntingShield != null)
        {
            Debug.Log($"⚠️ {name} 被【嘲讽】吸引，强制攻击 {tauntingShield.name}！");
            ExecuteAttack(tauntingShield.gameObject);
            isInAttackMode = false;
            return;
        }

        var allTargets = FindObjectsOfType<MonoBehaviour>()
            .Where(x => x is ShieldGuard || x is Knight)
            .Select(x => x.gameObject)
            .ToList();

        foreach (var go in allTargets)
        {
            if (go == gameObject) continue;
            var pos = go.GetComponent<ShieldGuard>()?.position ?? go.GetComponent<Knight>()?.position;
            if (pos.Equals(targetCoord))
            {
                ExecuteAttack(go);
                isInAttackMode = false;
                break;
            }
        }
    }

    void ExecuteAttack(GameObject target)
    {
        int myAtk = number;
        int targetAtk = GetNumber(target);

        number -= targetAtk;
        SetNumber(target, targetAtk - myAtk);

        Debug.Log($"[骑士攻击] {name} 攻击 {target.name}，互减 {myAtk}/{targetAtk}");

        if (GetNumber(target) > 0)
            TryRetaliate(target);

        if (number <= 0) Destroy(gameObject);
        if (GetNumber(target) <= 0) Destroy(target);
    }

    void TryRetaliate(GameObject target)
    {
        var s = target.GetComponent<ShieldGuard>();
        var k = target.GetComponent<Knight>();
        if (s != null)
            s.Retaliate(gameObject);
        else if (k != null)
            k.Retaliate(gameObject);
    }

    public void Retaliate(GameObject attacker)
    {
        int dmg = number;
        int atkNum = GetNumber(attacker);
        SetNumber(attacker, atkNum - dmg);

        Debug.Log($"[骑士反击] {name} 对 {attacker.name} 造成反击 {dmg}");

        if (GetNumber(attacker) <= 0)
            Destroy(attacker);
    }

    // 冲刺技能
    void ActivateDash()
    {
        isDashing = true;
        dashCooldown = dashCDMax;

        Debug.Log($"⚡ {name} 发动【冲刺】！(目前为测试触发)");

        // 可在此加入未来物理推进/击退逻辑
        Invoke(nameof(EndDash), 1.5f);
    }

    void EndDash() => isDashing = false;

    // 辅助函数
    int GetNumber(GameObject obj)
    {
        var s = obj.GetComponent<ShieldGuard>();
        if (s != null) return s.number;
        var k = obj.GetComponent<Knight>();
        if (k != null) return k.number;
        return 0;
    }

    void SetNumber(GameObject obj, int value)
    {
        var s = obj.GetComponent<ShieldGuard>();
        if (s != null) s.number = value;
        var k = obj.GetComponent<Knight>();
        if (k != null) k.number = value;
    }

    public void CollectTileValue(int value) => number += value;
}

using System.Linq;
using UnityEngine;
using static HexMath;
using static MovingObject;

public class ShieldGuard : MonoBehaviour
{
    [Header("基础属性")]
    public int number = 3;
    public Coordinates position;
    public int attackArea = 1;
    public bool isInAttackMode = false;
    public bool isTauntActive = false;

    [Header("系统引用")]
    public UnitInfoPanelController panel;
    public Renderer rend;

    [Header("技能参数")]
    private int tauntCooldown = 0;
    private const int tauntCDMax = 3; // 冷却3秒（用于手动测试）

    void Start()
    {
        if (panel == null)
            panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault();
    }

    void Update()
    {
        // 棋子被选中时才响应
        if (SelectionManager.selectedObj != gameObject) return;

        // 右键关闭UI
        if (panel != null && Input.GetMouseButtonDown(1))
            panel.Hide();

        // 攻击模式切换
        if (Input.GetKeyDown(KeyCode.X))
        {
            isInAttackMode = !isInAttackMode;
            if (isInAttackMode) ShowAttackableTiles();
            else ResetTiles();
        }

        // 攻击逻辑
        if (isInAttackMode && Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        // 技能：嘲讽
        if (Input.GetKeyDown(KeyCode.C) && tauntCooldown == 0)
        {
            ActivateTaunt();
        }

        // 冷却倒计时
        if (tauntCooldown > 0)
            tauntCooldown--;
    }

    void OnMouseDown()
    {
        SelectionManager.selectedObj = gameObject;
        if (panel != null)
            panel.ShowUnit("ShieldGuard", number);
    }

    // 攻击检测
    void TryAttack()
    {
        Vector3 mouseWorldPos = GridBuildingSystem.GetMousePos();
        Coordinates targetCoord = HexMath.WorldToCoordinates(mouseWorldPos, 1f);

        var allTargets = FindObjectsOfType<MonoBehaviour>()
            .Where(x => x is ShieldGuard || x is Knight)
            .Select(x => x.gameObject)
            .ToList();

        foreach (var go in allTargets)
        {
            if (go == gameObject) continue;
            var targetPos = go.GetComponent<ShieldGuard>()?.position ?? go.GetComponent<Knight>()?.position;
            if (targetPos.Equals(targetCoord))
            {
                ExecuteAttack(go);
                ResetTiles();
                isInAttackMode = false;
                break;
            }
        }
    }

    void ExecuteAttack(GameObject target)
    {
        int dmgToTarget = Mathf.CeilToInt(number * 0.5f);  // 笨重：输出减半
        int dmgToSelf = Mathf.CeilToInt((GetNumber(target)) * 0.5f);  // 减伤：受伤减半

        number -= dmgToSelf;
        SetNumber(target, GetNumber(target) - dmgToTarget);

        Debug.Log($"[盾卫攻击] {name} 对 {target.name} 造成 {dmgToTarget} 伤害（自身-{dmgToSelf}）");

        // 若目标存活，触发反击
        if (GetNumber(target) > 0)
            TryRetaliate(target);

        // 检查死亡
        if (number <= 0) Destroy(gameObject);
        if (GetNumber(target) <= 0) Destroy(target);
    }

    void TryRetaliate(GameObject target)
    {
        var shield = target.GetComponent<ShieldGuard>();
        var knight = target.GetComponent<Knight>();
        if (shield != null)
        {
            shield.Retaliate(this.gameObject);
        }
        else if (knight != null)
        {
            knight.Retaliate(this.gameObject);
        }
    }

    // 反击逻辑
    public void Retaliate(GameObject attacker)
    {
        int retaliateDamage = Mathf.CeilToInt(number * 0.5f);
        int atkNum = GetNumber(attacker);
        SetNumber(attacker, atkNum - retaliateDamage);
        Debug.Log($"[盾卫反击] {name} 对 {attacker.name} 造成反击 {retaliateDamage}");

        if (GetNumber(attacker) <= 0)
            Destroy(attacker);
    }

    // 嘲讽技能
    void ActivateTaunt()
    {
        isTauntActive = true;
        tauntCooldown = tauntCDMax;
        Invoke(nameof(EndTaunt), 5f); // 持续5秒
        Debug.Log($"🛡️ {name} 发动【嘲讽】，强制敌人攻击自己！");
    }

    void EndTaunt()
    {
        isTauntActive = false;
        Debug.Log($"{name} 的【嘲讽】效果结束。");
    }

    // 显示攻击范围（仅测试）
    void ShowAttackableTiles() { /* 可留空或高亮测试 */ }
    void ResetTiles() { }

    // 数据辅助函数
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

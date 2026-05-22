using UnityEngine;

/// <summary>
/// 回血地块 — 单位首次踏上时恢复 HP，之后变为普通地块外观
/// </summary>
public class HealOnceTile : Tile
{
    [Header("回血相关")]
    [SerializeField] private int healAmount = 10;
    [SerializeField] private Material usedHealMaterial;

    private bool healUsed = false;
    private Renderer cachedRenderer;

    protected override void Awake()
    {
        tileType = TileType.HealOnce;
        cachedRenderer = GetComponent<Renderer>();
    }

    protected override void OnUnitEnter(UnitController unit)
    {
        if (healUsed) return;

        unit.Heal(healAmount);
        healUsed = true;

        if (usedHealMaterial != null)
            cachedRenderer.material = usedHealMaterial;

        Debug.Log($"{unit.name} 在回血地块上恢复了 {healAmount} HP");
    }

    public override void ResetTile()
    {
        base.ResetTile();

        // 如果回血已使用且存在专用材质，恢复使用后的材质
        if (healUsed && usedHealMaterial != null)
            cachedRenderer.material = usedHealMaterial;
    }
}

using UnityEngine;

/// <summary>
/// 地块基类 — 所有地块类型的公共行为
/// 子类通过 Awake() 设置各自的 tileType，并在需要时重写虚方法
/// </summary>
public abstract class Tile : MonoBehaviour
{
    [Header("外观")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;

    /// <summary>地块类型（由子类在 Awake 中设定，外部只读）</summary>
    public TileType tileType { get; protected set; }

    private bool isHighlighted = false;
    private GameManager gameManager;
    private UnitController selectedUnit;
    private Renderer rend;

    // ─── Unity 生命周期 ────────────────────────────────────────
    protected virtual void Awake()
    {
    }
    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = defaultMaterial;
        gameManager = GameManager.instance;
    }

    // ─── 交互控制（子类可重写） ──────────────────────────────────

    /// <summary>能否被高亮/点击（ObstacleTile 返回 false）</summary>
    protected virtual bool CanInteract() => true;

    private void OnMouseDown()
    {
        if (!CanInteract()) return;

        if (isHighlighted && selectedUnit != null)
            MoveSelectedUnit();
    }

    public void HighlightTile(UnitController unit)
    {
        if (!CanInteract()) return;

        selectedUnit = unit;
        isHighlighted = true;
        rend.material = highlightMaterial;
    }

    public virtual void ResetTile()
    {
        isHighlighted = false;
        selectedUnit = null;
        rend.material = defaultMaterial;
    }

    // ─── 子类可重写的钩子 ────────────────────────────────────────

    /// <summary>单位进入地块时触发（HealOnceTile 在此处回血）</summary>
    protected virtual void OnUnitEnter(UnitController unit) { }

    // ─── 内部逻辑 ───────────────────────────────────────────────

    private void MoveSelectedUnit()
    {
        if (selectedUnit == null) return;

        OnUnitEnter(selectedUnit);

        selectedUnit.MoveToTile(transform.position);
        ResetAllOtherTiles();
        gameManager.CheckPlayerTurnEnd();
    }

    private void ResetAllOtherTiles()
    {
        foreach (var tile in FindObjectsOfType<Tile>())
        {
            if (tile != this) tile.ResetTile();
        }
    }
}

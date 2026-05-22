using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 单位基类 — 所有玩家/敌人单位的公共逻辑。
/// 子类可重写虚方法实现各自特有的行为。
/// </summary>
public abstract class UnitController : MonoBehaviour
{
    [Header("基础属性")]
    public Unit unit;

    /// <summary>行动力（从 Unit.movement 读取）</summary>
    protected int movementRange = 3;
    public bool hasGoneThisTurn = false;
    public int lastUnitThing; // 0=none, 1=walked, 2=attacked

    protected GameManager gameManager;
    protected GridManager gridManager;

    private int maxHP;
    private int currentHP;

    [SerializeField] private Slider HPSlider;

    // ─── Unity 生命周期 ────────────────────────────────────────

    /// <summary>
    /// Awake 中完成初始化，确保 Instantiate 后数据立即可用。
    /// 子类重写时记得 base.Awake()。
    /// </summary>
    protected virtual void Awake()
    {
        gameManager = GameManager.instance;
        gridManager = FindObjectOfType<GridManager>();

        if (unit != null)
        {
            movementRange = unit.movement;
            maxHP = unit.health;
            currentHP = maxHP;
        }

        if (HPSlider != null)
        {
            HPSlider.maxValue = maxHP;
            HPSlider.value = currentHP;
        }
    }

    // ─── 移动相关 ─────────────────────────────────────────────

    /// <summary>用 BFS 计算可到达格子并高亮（子类可重写改变移动规则）</summary>
    public virtual void HighlightAvailableMoves()
    {
        if (gridManager == null) return;
        if (movementRange <= 0) return;

        int startX = Mathf.RoundToInt(transform.position.x);
        int startY = Mathf.RoundToInt(transform.position.y);
        Vector2Int start = new Vector2Int(startX, startY);

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();
        queue.Enqueue(start);
        distance[start] = 0;

        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDist = distance[current];

            foreach (Vector2Int dir in dirs)
            {
                Vector2Int next = current + dir;
                if (distance.ContainsKey(next)) continue;

                GameObject tileObj = gridManager.GetTile(next.x, next.y);
                if (tileObj == null) continue;

                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null || tile is ObstacleTile) continue;

                distance[next] = currentDist + 1;

                if (currentDist + 1 <= movementRange)
                {
                    tile.HighlightTile(this);
                    queue.Enqueue(next);
                }
            }
        }
    }

    /// <summary>移动到指定格子（被 Tile.OnMouseDown 调用）</summary>
    public void MoveToTile(Vector3 position)
    {
        transform.position = position;
        gameManager.EnableButtonOptions();
    }

    /// <summary>吸附到最近的网格位置</summary>
    public void MoveToClosestTile()
    {
        int nearestX = Mathf.RoundToInt(transform.position.x);
        int nearestY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector3(nearestX, nearestY, 0);
    }

    // ─── 战斗相关（子类可重写） ──────────────────────────────

    public virtual void TakeDamage(int damage)
    {
        currentHP -= damage;
        HPSlider.value = currentHP;
    }

    public virtual void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        HPSlider.value = currentHP;
    }

    // ─── 交互 ─────────────────────────────────────────────────

    void OnMouseDown()
    {
        gameManager.SelectUnit(this);
    }
}

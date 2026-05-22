using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{
    /// <summary>行动力（从 Unit.movement 读取）</summary>
    private int movementRange = 3;
    public bool hasGoneThisTurn = false;
    public int lastUnitThing;
    //0 = none
    //1 = walked
    //2 = attacked
    private GameManager gameManager;
    private GridManager gridManager;

    public Unit unit;

    private int maxHP;
    private int currentHP;

    [SerializeField] private Slider HPSlider;

    void Start()
    {
        gameManager = GameManager.instance;
        gridManager = FindObjectOfType<GridManager>();

        // 行动力从 Unit 数据中读取
        movementRange = unit.movement;

        maxHP = unit.health;
        currentHP = maxHP;

        HPSlider.maxValue = maxHP;
        HPSlider.value = currentHP;
    }

    /// <summary>用 BFS 计算可到达格子并高亮</summary>
    public void HighlightAvailableMoves()
    {
        if (gridManager == null) return;
        if (movementRange <= 0) return;

        int startX = Mathf.RoundToInt(transform.position.x);
        int startY = Mathf.RoundToInt(transform.position.y);
        Vector2Int start = new Vector2Int(startX, startY);

        // BFS
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();
        queue.Enqueue(start);
        distance[start] = 0;

        // 四方向邻居
        Vector2Int[] dirs = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDist = distance[current];

            foreach (Vector2Int dir in dirs)
            {
                Vector2Int next = current + dir;
                if (distance.ContainsKey(next)) continue; // 已访问

                GameObject tileObj = gridManager.GetTile(next.x, next.y);
                if (tileObj == null) continue; // 超出地图边界

                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null || tile is ObstacleTile) continue; // 障碍物不可达

                distance[next] = currentDist + 1;

                if (currentDist + 1 <= movementRange)
                {
                    tile.HighlightTile(this);
                    queue.Enqueue(next);
                }
            }
        }
    }

    public void MoveToTile(Vector3 position)
    {
        // Move the unit to the selected tile position
        transform.position = position;

        gameManager.EnableButtonOptions();
        
    }

    public void MoveToClosestTile() {

        // Calculate the closest grid position
        int nearestX = Mathf.RoundToInt(this.transform.position.x);
        int nearestY = Mathf.RoundToInt(this.transform.position.y);
        Vector3 closestGridPosition = new Vector3(nearestX, nearestY, 0);

        // Move the unit to the closest grid position
        transform.position = closestGridPosition;

    }

    void OnMouseDown()
    {
        gameManager.SelectUnit(this);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        HPSlider.value = currentHP;
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        HPSlider.value = currentHP;
    }
}

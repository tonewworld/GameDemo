using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("地图 CSV 数据")]
    [SerializeField] private TextAsset mapCsv; // 在 Inspector 中拖入 CSV 文件

    [Header("地块预设（按 TileType 顺序）")]
    [SerializeField] private GameObject obstaclePrefab;   // TileType.Obstacle (0)
    [SerializeField] private GameObject walkablePrefab;   // TileType.Walkable (1)
    [SerializeField] private GameObject healOncePrefab;   // TileType.HealOnce  (2)

    private GameObject[,] grid;

    void Start()
    {
        GenerateMapFromCsv();
        MoveAllUnitsToClosestTile();
    }

    /// <summary>
    /// 从 CSV 读取地图数据并生成网格
    /// </summary>
    void GenerateMapFromCsv()
    {
        if (mapCsv == null)
        {
            Debug.LogError("[GridManager] mapCsv 未赋值！请将 CSV 文件拖入 Inspector。");
            return;
        }

        string[] lines = mapCsv.text.Split('\n');
        List<int[]> parsedRows = new List<int[]>();

        // 解析 CSV：逐行，跳过空行和注释行（以 # 开头）
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                continue;

            // 去掉可能存在的回车符 \r
            line = line.Replace("\r", "");

            string[] cells = line.Split(',');
            int[] row = new int[cells.Length];

            for (int i = 0; i < cells.Length; i++)
            {
                if (int.TryParse(cells[i].Trim(), out int val))
                    row[i] = val;
                else
                    row[i] = 0; // 解析失败默认为障碍物
            }

            parsedRows.Add(row);
        }

        if (parsedRows.Count == 0)
        {
            Debug.LogError("[GridManager] CSV 文件为空或格式错误！");
            return;
        }

        int gridSizeY = parsedRows.Count;
        int gridSizeX = parsedRows[0].Length;

        // 检查每行列数是否一致
        for (int y = 0; y < gridSizeY; y++)
        {
            if (parsedRows[y].Length != gridSizeX)
            {
                Debug.LogWarning($"[GridManager] CSV 第 {y} 行列数不一致，已截断/补零处理");
            }
        }

        // 销毁旧的网格（如有）
        if (grid != null)
        {
            foreach (GameObject tile in grid)
            {
                if (tile != null)
                    DestroyImmediate(tile);
            }
        }

        grid = new GameObject[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                int tileValue = (y < parsedRows.Count && x < parsedRows[y].Length)
                    ? parsedRows[y][x]
                    : 0;

                CreateTile(x, y, tileValue);
            }
        }

        Debug.Log($"[GridManager] 地图生成完成：{gridSizeX} x {gridSizeY}");
    }

    /// <summary>
    /// 根据 CSV 数值创建对应类型的地块
    /// </summary>
    void CreateTile(int x, int y, int tileValue)
    {
        TileType type = (TileType)tileValue;
        GameObject prefab = GetPrefabForType(type);

        if (prefab == null)
        {
            Debug.LogWarning($"[GridManager] ({x},{y}) 缺少 TileType.{type} 的预设，使用 Walkable 代替");
            prefab = walkablePrefab;

            if (prefab == null)
            {
                Debug.LogError("[GridManager] walkablePrefab 也未赋值！无法生成地图。");
                return;
            }
        }

        GameObject tileObj = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
        tileObj.name = $"Tile_{x}_{y}_{type}";
        grid[x, y] = tileObj;

        // 验证 Prefab 上确有 Tile 组件（类型已在子类 Awake 中自动设置）
        if (tileObj.GetComponent<Tile>() == null)
            Debug.LogError($"[GridManager] Prefab {prefab.name} 缺少 Tile 组件！");
    }

    /// <summary>
    /// 根据 TileType 返回对应的 Prefab
    /// </summary>
    GameObject GetPrefabForType(TileType type)
    {
        switch (type)
        {
            case TileType.Obstacle: return obstaclePrefab;
            case TileType.Walkable: return walkablePrefab;
            case TileType.HealOnce: return healOncePrefab;
            default: return walkablePrefab;
        }
    }

    /// <summary>
    /// 获取指定坐标的地块
    /// </summary>
    public GameObject GetTile(int x, int y)
    {
        if (grid == null) return null;
        if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
            return null;
        return grid[x, y];
    }

    /// <summary>
    /// 获取网格尺寸
    /// </summary>
    public Vector2Int GetGridSize()
    {
        if (grid == null) return Vector2Int.zero;
        return new Vector2Int(grid.GetLength(0), grid.GetLength(1));
    }

    void MoveAllUnitsToClosestTile()
    {
        UnitController[] units = FindObjectsOfType<UnitController>();

        foreach (UnitController unit in units)
        {
            unit.MoveToClosestTile();
        }
    }
}

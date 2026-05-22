using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 敌人单位基类 — 包含敌人 AI 的通用框架。
/// 不同兵种可继承此类并重写 <see cref="EnemyActionRoutine"/> 实现各自 AI。
/// </summary>
public class EnemyUnit : UnitController
{
    // ─── 事件 ─────────────────────────────────────────────────
    /// <summary>敌人行动完成时触发，GameManager 订阅此事件追踪回合进度</summary>
    public event Action OnEnemyActionCompleted;

    // ─── AI 接口 ─────────────────────────────────────────────

    /// <summary>
    /// 由 GameManager 调用，开始执行本回合行动。
    /// 启动 <see cref="EnemyActionRoutine"/> 协程。
    /// </summary>
    public void ExecuteEnemyTurn()
    {
        StartCoroutine(EnemyActionRoutine());
    }

    /// <summary>
    /// 敌人行动协程 — 虚方法，子类可重写实现不同 AI。
    /// 默认行为：随机移动到周围 4 格中一个可行走且未被占据的格子。
    /// 子类重写时必须在最后调用 <see cref="FinishEnemyAction"/>。
    /// </summary>
    protected virtual IEnumerator EnemyActionRoutine()
    {
        // —— 1. 移动 ——
        Vector3? target = FindRandomAdjacentTile();
        if (target.HasValue)
        {
            transform.position = target.Value;
        }

        hasGoneThisTurn = true;
        lastUnitThing = 1;

        // 小延迟让玩家看到动作
        yield return new WaitForSeconds(0.3f);

        // —— 2. 通知完成 ——
        FinishEnemyAction();
    }

    /// <summary>标记行动完成并触发事件（子类重写 EnemyActionRoutine 最后必须调用此方法）</summary>
    protected void FinishEnemyAction()
    {
        OnEnemyActionCompleted?.Invoke();
    }

    // ─── AI 辅助方法（子类可直接复用） ────────────────────────

    /// <summary>在周围 4 格中随机选取一个可行走且未被占据的格子</summary>
    protected Vector3? FindRandomAdjacentTile()
    {
        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        int startX = Mathf.RoundToInt(transform.position.x);
        int startY = Mathf.RoundToInt(transform.position.y);

        List<Vector2Int> validTiles = new List<Vector2Int>();

        foreach (Vector2Int dir in dirs)
        {
            int nx = startX + dir.x;
            int ny = startY + dir.y;

            GameObject tileObj = gridManager.GetTile(nx, ny);
            if (tileObj == null) continue;

            Tile tile = tileObj.GetComponent<Tile>();
            if (tile == null || tile is ObstacleTile) continue;

            if (IsTileOccupied(nx, ny)) continue;

            validTiles.Add(new Vector2Int(nx, ny));
        }

        if (validTiles.Count == 0) return null;

        Vector2Int chosen = validTiles[UnityEngine.Random.Range(0, validTiles.Count)];
        return new Vector3(chosen.x, chosen.y, 0);
    }

    /// <summary>检查某个格子是否已被其他单位占据</summary>
    protected bool IsTileOccupied(int x, int y)
    {
        UnitController[] allUnits = FindObjectsOfType<UnitController>();
        foreach (UnitController u in allUnits)
        {
            if (u == this) continue;
            if (Mathf.RoundToInt(u.transform.position.x) == x &&
                Mathf.RoundToInt(u.transform.position.y) == y)
                return true;
        }
        return false;
    }
}

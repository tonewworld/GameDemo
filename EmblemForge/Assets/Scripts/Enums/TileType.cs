/// <summary>
/// 地块类型枚举
/// </summary>
public enum TileType
{
    /// <summary>不可到达的障碍物（如墙壁、深渊）</summary>
    Obstacle = 0,

    /// <summary>普通可行走地块</summary>
    Walkable = 1,

    /// <summary>回血地块（只能触发一次，触发后变为普通地块）</summary>
    HealOnce = 2,
}

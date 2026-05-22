using UnityEngine;

/// <summary>
/// 不可到达的障碍物地块（不可高亮、不可点击）
/// </summary>
public class ObstacleTile : Tile
{
    protected override void Awake()
    {
        tileType = TileType.Obstacle;
    }

    protected override bool CanInteract() => false;
}

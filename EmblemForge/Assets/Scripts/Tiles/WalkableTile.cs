using UnityEngine;

/// <summary>
/// 普通可行走地块
/// </summary>
public class WalkableTile : Tile
{
    protected override void Awake()
    {
        tileType = TileType.Walkable;
    }
}

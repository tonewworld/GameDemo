using UnityEngine;

/// <summary>
/// 单位生成条目 — 在 Inspector 中配置预制体和出生坐标
/// 注意：UnitSpawnEntry 作为纯序列化类不能包含 UnityEngine.Object 引用，
/// 否则 Unity UI Toolkit Inspector 会触发 SerializedObject disposed 错误。
/// 因此用平行数组代替：prefabs[i] + positions[i] 共同描述一个出生点。
/// </summary>
[System.Serializable]
public struct UnitSpawnEntry
{
    public Vector2Int gridPosition;    // 地图格子坐标 (x, y)
}

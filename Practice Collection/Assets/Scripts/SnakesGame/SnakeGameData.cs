using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 网格单元格类型
public enum CellType
{
    Empty,      // 空可通行
    Obstacle,   // 带叉障碍物，不可通行
    TargetGreen,// 绿色目标格
    TargetYellow,// 黄色目标格
    TargetRed,  // 红色目标格
    TargetBlue  // 蓝色目标格
}

[System.Serializable]
public class SnakeNodeData
{
    public Vector2 gridPos; // 网格坐标
    public NodeType nodeType;  // 节点类型（对应资源）
    public bool isHead;        // 是否是蛇头（可拖动）
    public bool isTail;        // 是否是蛇尾（可拖动）
}

public enum NodeType
{
    Red,    // red 红圈节点
    Blue,   // blue 蓝圈节点
    Green,  // green 绿圈节点
    Yellow, // yellow 黄圈节点
    Gray,   // gray 灰圈节点
    Black   // black 纯黑节点
}

[CreateAssetMenu(fileName = "SnakeLevelData", menuName = "Game/Snake Level Data")]
public class SnakeLevelData : ScriptableObject
{
    public float gridWidth = 8;
    public float gridHeight = 8;
    public List<SnakeNodeData> initialSnakeNodes = new List<SnakeNodeData>();
}
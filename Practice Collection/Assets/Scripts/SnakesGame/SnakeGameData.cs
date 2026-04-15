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

public class SnakeNodeData
{
    public Vector2Int gridPos;
    public Color dotColor;
    public bool isHead;
    public bool isTail;
}
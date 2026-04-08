using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画优先级
/// </summary>
public enum AnimationPriority
{
    None = -1,
    Background = 0,
    Low = 1,
    Normal = 2,
    High = 3,
    Critical = 4,
    Highest = 5
}
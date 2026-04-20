using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 网格单元格类型

public class Grid : MonoBehaviour
{
    public static Grid Instance;

    [Header("网格配置")]
    public float cellSize = 1.2f;
    public Vector2 gridOrigin = new Vector2(-4.8f, 4.9f); // 8x8网格左上角

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 网格坐标 → 世界坐标
    public Vector3 GridToWorld(Vector2 gridPos)
    {
        float x = gridOrigin.x + gridPos.x * cellSize;
        float y = gridOrigin.y - gridPos.y * cellSize;
        return new Vector3(x, y, 0);
    }

    // 世界坐标 → 网格坐标
    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - gridOrigin.x) / cellSize);
        int y = Mathf.RoundToInt((gridOrigin.y - worldPos.y) / cellSize);
        return new Vector2Int(x, y);
    }
}

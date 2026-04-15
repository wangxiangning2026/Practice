using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 网格单元格类型

public class Grid : MonoBehaviour
{
    public static Grid instance;
    public float cellSize = 1f;
    public Vector2 origin;

    [Header("Sprites")]
    public Sprite empty;
    public Sprite obstacle;
    public Sprite redTarget;
    public Sprite greenTarget;
    public Sprite blueTarget;
    public Sprite yellowTarget;

    //private LevelData currentLevel;

    void Awake() => instance = this;

    // 世界坐标 → 网格坐标
    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y - origin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    // 网格坐标 → 世界坐标
    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        return new Vector2(
            origin.x + gridPos.x * cellSize,
            origin.y + gridPos.y * cellSize
        );
    }

    // 生成地图（纯Sprite）
    // public void GenerateLevel(LevelData level)
    // {
    //     currentLevel = level;
    //     ClearAllChildren();
    //
    //     for (int x = 0; x < level.width; x++)
    //     {
    //         for (int y = 0; y < level.height; y++)
    //         {
    //             var type = level.grid[y * level.width + x];
    //             CreateCell(x, y, type);
    //         }
    //     }
    // }

    void CreateCell(int x, int y, CellType type)
    {
        GameObject go = new GameObject($"Cell_{x}_{y}");
        go.transform.parent = transform;
        go.transform.position = GridToWorld(new Vector2Int(x, y));

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSprite(type);
        sr.sortingOrder = 0;
    }

    Sprite GetSprite(CellType type) => type switch
    {
        CellType.Obstacle => obstacle,
        CellType.TargetRed => redTarget,
        CellType.TargetGreen => greenTarget,
        CellType.TargetBlue => blueTarget,
        CellType.TargetYellow => yellowTarget,
        _ => empty
    };

    void ClearAllChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
    }

    // public bool IsValidCell(Vector2Int pos)
    // {
    //     if (pos.x < 0 || pos.x >= currentLevel.width || pos.y < 0 || pos.y >= currentLevel.height)
    //         return false;
    //
    //     var type = currentLevel.grid[pos.y * currentLevel.width + pos.x];
    //     return type != CellType.Obstacle;
    // }
}

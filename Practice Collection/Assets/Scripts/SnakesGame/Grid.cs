using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("格子设置")]
    public float cellSize = 1.2f;
    public Vector2 gridOrigin = new Vector2(-4.22f, -2.82f);
    public int width  = 8;
    public int height = 7;

    // 世界坐标 → 格子坐标
    public Vector2Int WorldToGridPos(Vector3 worldPos)
    {
        float fx = (worldPos.x - gridOrigin.x) / cellSize;
        float fy = (worldPos.y - gridOrigin.y) / cellSize;

        int gx = Mathf.FloorToInt(fx);
        int gy = Mathf.FloorToInt(fy);

        gx = Mathf.Clamp(gx, 0, width - 1);
        gy = Mathf.Clamp(gy, 0, height - 1);

        return new Vector2Int(gx, gy);
    }

    // 格子坐标 → 世界中心（蛇正好站格子里）
    public Vector3 GridToWorldPos(Vector2Int gridPos)
    {
        float x = gridOrigin.x + gridPos.x * cellSize + cellSize * 0.5f;
        float y = gridOrigin.y + gridPos.y * cellSize + cellSize * 0.5f;
        return new Vector3(x, y, 0f);
    }
}

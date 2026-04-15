using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMove : MonoBehaviour
{
    public static SnakeMove instance;
    //public LevelData currentLevel;
    public GameObject nodePrefab;
    public List<SnakeNode> nodes = new();

    void Awake() => instance = this;

    // 生成蛇
    // public void SpawnSnake()
    // {
    //     ClearNodes();
    //     foreach (var data in currentLevel.snakeNodes)
    //     {
    //         var go = Instantiate(nodePrefab, transform);
    //         var node = go.GetComponent<SnakeNode>();
    //         node.Init(this, data);
    //         nodes.Add(node);
    //     }
    // }

    // 尝试移动头/尾
    public void TryMove(SnakeNode dragNode, Vector2Int targetPos)
    {
        //if (!GridManager.instance.IsValidCell(targetPos)) return;
        if (IsOnSnake(targetPos)) return;

        if (dragNode.data.isHead)
            MoveHead(targetPos);
        else if (dragNode.data.isTail)
            MoveTail(targetPos);

        // if (CheckWin())
        //     Debug.Log("✅ 关卡通关！");
    }

    // 移动头 → 身体依次跟进
    void MoveHead(Vector2Int newHeadPos)
    {
        Vector2Int prev = nodes[0].data.gridPos;
        nodes[0].SetPosition(newHeadPos);

        for (int i = 1; i < nodes.Count; i++)
        {
            Vector2Int temp = nodes[i].data.gridPos;
            nodes[i].SetPosition(prev);
            prev = temp;
        }
    }

    // 移动尾 → 身体依次跟进
    void MoveTail(Vector2Int newTailPos)
    {
        Vector2Int prev = nodes[^1].data.gridPos;
        nodes[^1].SetPosition(newTailPos);

        for (int i = nodes.Count - 2; i >= 0; i--)
        {
            Vector2Int temp = nodes[i].data.gridPos;
            nodes[i].SetPosition(prev);
            prev = temp;
        }
    }

    // 检测是否在蛇身上
    bool IsOnSnake(Vector2Int pos)
    {
        foreach (var n in nodes)
            if (n.data.gridPos == pos)
                return true;
        return false;
    }

    // 胜利条件：颜色圆点都在对应目标格
    // public bool CheckWin()
    // {
    //     foreach (var node in nodes)
    //     {
    //         if (node.data.dotColor == Color.clear) continue;
    //
    //         Vector2Int p = node.data.gridPos;
    //         CellType cell = currentLevel.grid[p.y * currentLevel.width + p.x];
    //         CellType target = ColorToCell(node.data.dotColor);
    //
    //         if (cell != target) return false;
    //     }
    //     return true;
    // }

    CellType ColorToCell(Color c)
    {
        if (c == Color.red) return CellType.TargetRed;
        if (c == Color.green) return CellType.TargetGreen;
        if (c == Color.blue) return CellType.TargetBlue;
        if (c == Color.yellow) return CellType.TargetYellow;
        return CellType.Empty;
    }

    void ClearNodes()
    {
        foreach (var n in nodes) Destroy(n.gameObject);
        nodes.Clear();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    public SnakeNodeData data;
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer dotRenderer;
    public bool isDragging;

    private SnakeMove snake;

    public void Init(SnakeMove snake, SnakeNodeData data)
    {
        this.snake = snake;
        this.data = data;
        dotRenderer.color = data.dotColor;
        transform.position = Grid.instance.GridToWorld(data.gridPos);
    }

    // 鼠标按下
    void OnMouseDown()
    {
        if (!data.isHead && !data.isTail) return;
        isDragging = true;
    }

    // 鼠标拖动
    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int targetGrid = Grid.instance.WorldToGrid(mouseWorld);

        if (targetGrid == data.gridPos) return;
        if (Mathf.Abs(targetGrid.x - data.gridPos.x) + Mathf.Abs(targetGrid.y - data.gridPos.y) != 1)
            return; // 只允许上下左右一格

        snake.TryMove(this, targetGrid);
    }

    // 鼠标抬起
    void OnMouseUp() => isDragging = false;

    // 移动到目标网格
    public void SetPosition(Vector2Int gridPos)
    {
        data.gridPos = gridPos;
        transform.position = Grid.instance.GridToWorld(gridPos);
    }
}

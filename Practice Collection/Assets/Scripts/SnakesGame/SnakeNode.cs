using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    [Header("节点数据")]
    public SnakeNodeData nodeData;
    public SpriteRenderer sr;
    public bool isHead;
    public bool isTail;

    private Snake _snake;
    private bool _isDragging;

    // 初始化节点
    public void Init(Snake snake, SnakeNodeData data, Sprite nodeSprite)
    {
        _snake = snake;
        nodeData = data;
        sr.sprite = nodeSprite;
        transform.position = Grid.Instance.GridToWorld(data.gridPos);
    }

    // 鼠标按下：只有头/尾可拖动
    private void OnMouseDown()
    {
        if (!nodeData.isHead && !nodeData.isTail) return;
        _isDragging = true;
    }

    // 鼠标拖动：只允许上下左右一格移动
    private void OnMouseDrag()
    {
        if (!_isDragging) return;
        
        // 屏幕坐标转世界坐标，再转网格坐标
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int targetGridPos = Grid.Instance.WorldToGrid(mouseWorldPos);

        // 只允许相邻格子（曼哈顿距离=1，禁止斜向）
        if (!Mathf.Approximately(Vector2.Distance(targetGridPos, nodeData.gridPos), 1))
            return;

        // 通知蛇移动
        if (nodeData.isHead)
            _snake.TryMoveHead(targetGridPos);
        else
            _snake.TryMoveTail(targetGridPos);
    }

    // 鼠标抬起：结束拖动
    private void OnMouseUp()
    {
        _isDragging = false;
    }

    // 更新节点位置（蛇移动时调用）
    public void UpdatePosition(Vector2 newGridPos)
    {
        nodeData.gridPos = newGridPos;
        transform.position = Grid.Instance.GridToWorld(newGridPos);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMove : MonoBehaviour
{
    [Header("核心设置")] public GameObject nodePrefab;
    public int bodyLength = 5; // 身体节点数（不含头尾）
    public LineRenderer lineRenderer;

    private List<SnakeNode> nodeList = new List<SnakeNode>();
    private int selectedNodeIndex = -1;
    private Vector3 mouseOffset;

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectNode();
        }
        else if (Input.GetMouseButton(0) && selectedNodeIndex != -1)
        {
            DragNode();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectedNodeIndex = -1;
        }
    }

    // 选中节点（通过射线检测）
    void SelectNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SnakeNode node = hit.collider.GetComponent<SnakeNode>();
            if (node != null && node.isHead)
            {
                selectedNodeIndex = node.Index;
                mouseOffset = hit.point - hit.collider.transform.position;
            }
        }
    }

    // 拖动节点
    void DragNode()
    {
        // 1. 获取鼠标世界位置并转换为网格坐标
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0; // 2D游戏
        Vector2Int targetGridPos = Grid.Instance.WorldToGridPos(mouseWorld);

        // 2. 计算目标世界位置（格子中心）
        Vector3 targetWorldPos = Grid.Instance.GridToWorldPos(targetGridPos);

        // 3. 移动选中的节点
        nodeList[selectedNodeIndex].transform.position = targetWorldPos - mouseOffset;

        // 4. 关键逻辑：身体跟随
        // 如果拖动的是头，身体跟着头动
        if (selectedNodeIndex == 0)
        {
            for (int i = 1; i < nodeList.Count; i++)
            {
                Vector3 targetPos = nodeList[i - 1].transform.position;
                // 保持固定距离移动
                nodeList[i].transform.position =
                    Vector3.Lerp(nodeList[i].transform.position, targetPos, Time.deltaTime * 10f);
            }
        }
        // 如果拖动的是尾，身体倒着跟动（模拟整条蛇拖动）
        else if (selectedNodeIndex == nodeList.Count - 1)
        {
            for (int i = nodeList.Count - 2; i >= 0; i--)
            {
                Vector3 targetPos = nodeList[i + 1].transform.position;
                nodeList[i].transform.position =
                    Vector3.Lerp(nodeList[i].transform.position, targetPos, Time.deltaTime * 10f);
            }
        }

        // 5. 检查颜色匹配（如果需要）
        //CheckColorMatch();
    }
}
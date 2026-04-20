using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
  public static Snake Instance;

    [Header("配置")]
    public SnakeLevelData levelData;
    public float cellSize = 1f;
    public LineRenderer lineRenderer; // 黑色连线（用你的line资源做材质）

    [Header("节点预制体")]
    public GameObject redNodePrefab;
    public GameObject blueNodePrefab;
    public GameObject greenNodePrefab;
    public GameObject yellowNodePrefab;
    public GameObject grayNodePrefab;
    public GameObject blackNodePrefab;

    public List<SnakeNode> snakeNodes = new List<SnakeNode>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnSnake();
        InitLineRenderer();
    }

    // 生成整条蛇
    public void SpawnSnake()
    {
        // 清除旧节点
        foreach (var node in snakeNodes)
            Destroy(node.gameObject);
        snakeNodes.Clear();
    
        // 按配置生成新节点
        foreach (var nodeData in levelData.initialSnakeNodes)
        {
            GameObject prefab = GetNodePrefab(nodeData.nodeType);
            var nodeObj = Instantiate(prefab, transform);
            var node = nodeObj.GetComponent<SnakeNode>();
            
            // 初始化节点
            node.Init(this, nodeData, prefab.GetComponent<SpriteRenderer>().sprite);
            snakeNodes.Add(node);
        }
    }

    // 根据节点类型获取对应预制体
    private GameObject GetNodePrefab(NodeType type)
    {
        return type switch
        {
            NodeType.Red => redNodePrefab,
            NodeType.Blue => blueNodePrefab,
            NodeType.Green => greenNodePrefab,
            NodeType.Yellow => yellowNodePrefab,
            NodeType.Gray => grayNodePrefab,
            NodeType.Black => blackNodePrefab,
            _ => blackNodePrefab
        };
    }

    // 初始化黑线（LineRenderer）
    private void InitLineRenderer()
    {
        // 用你的line资源做材质
        lineRenderer.material = blackNodePrefab.GetComponent<SpriteRenderer>().material;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.8f; // 适配你的格子大小
        lineRenderer.endWidth = 0.8f;
        lineRenderer.positionCount = snakeNodes.Count;
        UpdateLine();
    }

    // 更新黑线位置（蛇移动后调用）
    private void UpdateLine()
    {
        for (int i = 0; i < snakeNodes.Count; i++)
        {
            lineRenderer.SetPosition(i, snakeNodes[i].transform.position);
        }
    }

    // 移动蛇头（核心逻辑）
    public bool TryMoveHead(Vector2Int newHeadPos)
    {
        // 1. 检查位置合法性（不越界、不撞蛇身、不撞障碍物）
        // if (!IsPositionValid(newHeadPos))
        //     return false;

        // 2. 移动头节点
        var head = snakeNodes[0];
        var oldHeadPos = head.nodeData.gridPos;
        head.UpdatePosition(newHeadPos);

        // 3. 依次移动后续节点（每个节点移动到前一个的旧位置）
        for (int i = 1; i < snakeNodes.Count; i++)
        {
            var prevNode = snakeNodes[i - 1];
            var currNode = snakeNodes[i];
            var oldPos = currNode.nodeData.gridPos;
            currNode.UpdatePosition(prevNode.nodeData.gridPos);
            prevNode.nodeData.gridPos = oldPos;
        }

        // 4. 更新黑线
        UpdateLine();
        return true;
    }

    // 移动蛇尾（核心逻辑）
    public bool TryMoveTail(Vector2Int newTailPos)
    {
        // 1. 检查位置合法性
        // if (!IsPositionValid(newTailPos))
        //     return false;

        // 2. 移动尾节点
        var tail = snakeNodes[snakeNodes.Count - 1];
        var oldTailPos = tail.nodeData.gridPos;
        tail.UpdatePosition(newTailPos);

        // 3. 依次移动前面的节点（每个节点移动到后一个的旧位置）
        for (int i = snakeNodes.Count - 2; i >= 0; i--)
        {
            var nextNode = snakeNodes[i + 1];
            var currNode = snakeNodes[i];
            var oldPos = currNode.nodeData.gridPos;
            currNode.UpdatePosition(nextNode.nodeData.gridPos);
            nextNode.nodeData.gridPos = oldPos;
        }

        // 4. 更新黑线
        UpdateLine();
        return true;
    }

    // 位置合法性检查
    // private bool IsPositionValid(Vector2Int pos)
    // {
    //     // 1. 越界检查
    //     if (pos.x < 0 || pos.x >= levelData.gridWidth || pos.y < 0 || pos.y >= levelData.gridHeight)
    //         return false;
    //
    //     // 2. 蛇身重叠检查（不能撞自己）
    //     foreach (var node in snakeNodes)
    //     {
    //         if (node.nodeData.gridPos == pos)
    //             return false;
    //     }
    //
    //     // 3. 障碍物检查（C3Aforbidingbox，带叉的格子）
    //     // 这里可以根据你的地图数据扩展，先做基础版
    //     return true;
    // }
}

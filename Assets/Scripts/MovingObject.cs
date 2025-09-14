using UnityEngine;
using UnityEngine.UIElements;
using static HexMath;
using static HexTile;
public class MovingObject : MonoBehaviour
{
    public TurnManager turnManager;
    public static GameObject selectedObj; // 当前选中的物体
    public Coordinates coords; // 棋子原坐标(using HexMath)
    private static Coordinates currentCoords; // 指针坐标
    private Vector3 originPosition; // 棋子的原transform位置
    private Vector3 position; // 棋子的transform位置
    private Vector3 currentPosition; // 指针的transform位置
    private bool isDragging = false; // 是否正在拖动物体
    private bool isObjectSelected = false; // 标记物体是否已经选中
    private Player player;  // 当前玩家
    public Vector3 centerWorld; //Test

    // LineRenderer 用来绘制路径
    private LineRenderer lineRenderer;

    // 记录玩家的行动点
    private int currentAP;


    void Start()
    {
        // 初始化 LineRenderer 组件
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;  // 初始时没有任何点
        lineRenderer.startWidth = 0.1f;  // 设置路径线的宽度
        lineRenderer.endWidth = 0.1f;    // 设置路径线的宽度
        turnManager = FindObjectOfType<TurnManager>(); // 获取 TurnManager 实例
        // player = new Player(1, "name"); // 测试用
    }


    void Update()
    {
        // 初始化 Player
        player = turnManager.players[turnManager.turnCount];

        // 如果鼠标左键按下，选中物体
        if (Input.GetMouseButtonDown(0))
        {
            // 执行射线检测，获取点击的物体
            GameObject clickedObject = RayToGetObj();
            if (clickedObject == null) return;

            // 检查是否有PlacebleObject脚本
            PlacebleObject placebleObject = clickedObject.GetComponent<PlacebleObject>();
            if (placebleObject == null) return;

            // 如果当前有物体被选中，取消选中并取消高亮
            if (isObjectSelected && selectedObj != clickedObject)
            {
                Unhighlight();
                isObjectSelected = false;
            }

            // 如果之前没有选中物体，选中当前物体
            if (!isObjectSelected)
            {
                selectedObj = clickedObject;
                isObjectSelected = true;

                Highlight();

                originPosition = selectedObj.transform.position;
                coords = HexMath.WorldToCoordinates(selectedObj.transform.position, HexTile.radius);
                Debug.Log($"选中时物体坐标: ({coords.x}, {coords.z})");

                // 开始绘制路径
                lineRenderer.positionCount = 1; // 从起始位置开始
                lineRenderer.SetPosition(0, originPosition); // 设置第一个位置
            }
            // 如果物体已选中并且已经在拖动，点击时将物体放下
            else if (isDragging)
            {
                // 将物体停在当前的位置，但保持物体选中状态
                isDragging = false;
                Debug.Log("物体已放下");

                ClearPath();
            }
        }

        // 右键点击取消高亮并取消选中物体
        if (Input.GetMouseButtonDown(1))
        {
            // 如果物体正在拖动，右键点击时返回原始位置并取消拖动
            if (isDragging)
            {
                selectedObj.transform.position = originPosition; // 将物体返回到原始位置
                player.actionPoints = currentAP; // 恢复行动点

                Unhighlight();
                isDragging = false;
                isObjectSelected = false;

                ClearPath();
            }
            else if (isObjectSelected)
            {
                // 如果物体没有在拖动状态下右键点击，取消高亮
                Unhighlight();
                isObjectSelected = false;
                selectedObj = null;

                ClearPath();
            }
        }

        // 模式切换时避免冲突
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.V))
        {
            // 如果物体正在拖动，切换模式返回原始位置并取消拖动，但是保持选中
            if (isDragging)
            {
                selectedObj.transform.position = originPosition; // 将物体返回到原始位置
                player.actionPoints = currentAP; // 恢复行动点
                isDragging = false; // 取消拖动状态
                ClearPath();
            }
        }

        // 按下 Z 键开始拖动
        if (isObjectSelected && Input.GetKeyDown(KeyCode.Z))
        {
            originPosition = selectedObj.transform.position;
            if (player.actionPoints > 0)  // 检查玩家是否有足够的行动点
            {
                currentAP = player.actionPoints; // 记录当前行动点
                isDragging = true;
                Debug.Log("开始拖动物体，剩余行动点: " + currentAP);
            }
            else
            {
                Debug.Log("没有足够的行动点！");
            }
        }

        // 如果正在拖动物体
        if (isDragging && player.actionPoints > 0)
        {
            // 获取鼠标位置并将物体拖动到该位置，同时使用 Snap 函数来对齐到网格
            selectedObj.transform.position = Snap(transform.position);

            //拖拽前，获得当前物体的坐标

            // 记录当前位置
            currentCoords = HexMath.WorldToCoordinates(selectedObj.transform.position, HexTile.radius);
            currentPosition = selectedObj.transform.position;

            Debug.Log($"拖拽前物体坐标: ({currentCoords.x}, {currentCoords.z})");
            if (HasMovedALot(coords, currentCoords))
            {
                selectedObj.transform.position = position;
            }
            else if (HasMovedOneStep(coords, currentCoords))
            {
                player.actionPoints--;  // 每次走一格消耗 1 点行动点
                coords = currentCoords;  // 更新起始坐标
                position = currentPosition;
            }
            UpdatePath(selectedObj.transform.position);
        }
    }


    // Method
    // 执行射线检测
    public GameObject RayToGetObj()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从鼠标位置发射射线
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // 如果射线击中了物体
        {
            return hit.collider.gameObject; // 返回被点击的物体
        }

        return null; // 如果没有击中物体，返回 null
    }

    // 判断是否移动了（根据坐标差）
    private bool HasMovedOneStep(Coordinates start, Coordinates current)
    {
        int distance = Mathf.Max(Mathf.Abs(current.x - start.x), Mathf.Abs(current.z - start.z), Mathf.Abs((current.x - start.x) + (current.z - start.z)));
        return distance == 1;
    }

    private bool HasMovedALot(Coordinates start, Coordinates current)
    {
        int distance = Mathf.Max(Mathf.Abs(current.x - start.x), Mathf.Abs(current.z - start.z), Mathf.Abs((current.x - start.x) + (current.z - start.z)));
        return distance > 1;
    }

    // Snap 方法，通过调用 GridBuildingSystem 的 Snap 函数来实现对齐
    private Vector3 Snap(Vector3 rootPos)
    {
        return GridBuildingSystem.Instance.Snap(rootPos); // 调用 GridBuildingSystem 中的 Snap 函数
    }

    // 更新路径（在物体移动时绘制路径）
    private void UpdatePath(Vector3 currentPosition)
    {
        int currentPosCount = lineRenderer.positionCount;

        // 添加新的路径点
        lineRenderer.positionCount = currentPosCount + 1;
        lineRenderer.SetPosition(currentPosCount, currentPosition);
    }

    // 清除路径（物体放下或取消选中时调用）
    private void ClearPath()
    {
        lineRenderer.positionCount = 0; // 清空所有路径点
    }

    // 高亮物体
    private static void Highlight()
    {
        if (selectedObj != null)
        {
            Outline outline = selectedObj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = true; // 显示轮廓
            }
        }
    }

    // 取消高亮物体
    private static void Unhighlight()
    {
        if (selectedObj != null)
        {
            Outline outline = selectedObj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false; // 禁用轮廓
            }
            selectedObj = null; // 清空当前选中的物体
        }
    }
}

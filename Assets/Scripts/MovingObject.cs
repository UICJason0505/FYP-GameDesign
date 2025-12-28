
using UnityEngine;
using static HexMath;
using static HexTile;

public class MovingObject : MonoBehaviour
{
    public TurnManager turnManager;
    public static GameObject selectedObj; // 当前选中的物体
    public Coordinates coords; // 棋子原坐标(using HexMath)
    private static Coordinates currentCoords; // 指针坐标
    private Vector3 originPosition; // 棋子的原transform位置
    private int originalNumber; // 棋子的原数值
    private Vector3 lastPosition; // 棋子上一次合法的transform位置
    private Vector3 currentPosition; // 棋子现在的transform位置
    private Player player; // 当前玩家
    public Vector3 centerWorld; //Test
    private LineRenderer lineRenderer; // 用来绘制路径
    private int currentAP; // 记录玩家的行动点

    public enum ObjectState { None, Selected, Dragging }
    public ObjectState currentState = ObjectState.None;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        turnManager = FindObjectOfType<TurnManager>();
    }

    void Update()
    {
        player = turnManager.players[turnManager.turnCount];

        // 左键点击选中物体
        if (Input.GetMouseButtonDown(0)) HandleLeftClick();

        // 右键点击取消选中
        if (Input.GetMouseButtonDown(1)) HandleRightClick();

        // 按下 Z 键开始拖动
        if (currentState == ObjectState.Selected && Input.GetKeyDown(KeyCode.Z)) StartDragging();
        else if (currentState == ObjectState.Dragging && Input.GetKeyDown(KeyCode.Z)) ReturnToOriginalPosition();

        // 如果正在拖动物体
        if (currentState == ObjectState.Dragging && player.actionPoints > 0) HandleDragging();
    }

    // Left click handling
    private void HandleLeftClick()
    {
        // === FYJ：检测阵营是否允许操作 ===
        GameObject clickedObject = RayToGetObj();
        if (clickedObject == null) return;

        PlacebleObject placebleObject = clickedObject.GetComponent<PlacebleObject>();
        if (placebleObject == null) return;

        King clickedChess = clickedObject.GetComponent<King>();
        if (clickedChess != null)
        {
            Player clickedPlayer = clickedChess.player;
            Player currentPlayer = turnManager.players[turnManager.turnCount];

            if (clickedPlayer != currentPlayer)
            {
                Debug.Log($"当前回合属于 {currentPlayer.playerName}，无法操作 {clickedPlayer.playerName} 的棋子！");
                if (UnitInfoPanelController.Instance != null)
                    UnitInfoPanelController.Instance.ShowInvalidAction("Not your turn!");
                return;
            }
        }

        if (currentState == ObjectState.Selected && selectedObj != clickedObject)
        {
            Unhighlight();
            currentState = ObjectState.None;
        }

        if (currentState == ObjectState.None)
        {
            selectedObj = clickedObject;
            currentState = ObjectState.Selected;
            Highlight();
            var mr = selectedObj.GetComponent<Renderer>();
            originPosition = mr.bounds.center;
            coords = WorldToCoordinates(originPosition, HexTile.radius);

            // 初始化合法位置
            lastPosition = originPosition;

            Debug.Log($"选中时物体坐标: ({coords.x}, {coords.z})");

            // 开始绘制路径
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, originPosition);
        }

        // 放下物体（拖拽状态下点击左键）
        else if (currentState == ObjectState.Dragging)
        {
            Coordinates targetCoords = WorldToCoordinates(selectedObj.transform.position, HexTile.radius);
            Chess movingChess = selectedObj.GetComponent<Chess>();

            /** 实际上不需要这一段代码，因为正常情况下拖拽时不会将棋子放到被占用的地方上，但是加上会保险一点
            // === 用 allChess 判断是否有其他棋子占据目标格子 ===
            bool occupiedByOther = false;

            foreach (var other in HexTile.allChess)
            {
                if (other == null) continue;
                if (other == movingChess) continue;  // 自己不算占用

                if (other.position.x == targetCoords.x &&
                    other.position.z == targetCoords.z)
                {
                    occupiedByOther = true;
                    break;
                }
            }

            // 如果被别人占用 → 不允许放下
            if (occupiedByOther)
            {
                Debug.Log("该格子已被其他棋子占用，不能放下！");
                if (UnitInfoPanelController.Instance != null)
                    UnitInfoPanelController.Instance.ShowInvalidAction("Tile is occupied!");

                // 不结束拖拽，让玩家继续移动
                return;
            }
            **/


            // 可以放下
            currentState = ObjectState.None;
            Debug.Log("物体已放下");
            ClearPath();

            Vector3 fixedPos = selectedObj.transform.position;
            fixedPos.y = 0.375f;
            selectedObj.transform.position = fixedPos;
        }
    }

    // Right click handling
    private void HandleRightClick()
    {
        if (currentState == ObjectState.Dragging)
        {
            ResetPosition();
            ClearPath();
        }
        else if (currentState == ObjectState.Selected)
        {
            Unhighlight();
            currentState = ObjectState.None;
            selectedObj = null;
            ClearPath();
        }
    }

    // Start dragging object
    private void StartDragging()
    {
        if (player.actionPoints > 0)
        {
            currentAP = player.actionPoints;
            currentState = ObjectState.Dragging;
            Debug.Log("开始拖动物体，剩余行动点: " + currentAP);
            originalNumber = selectedObj.GetComponent<Chess>().number;
        }
        else
        {
            Debug.Log("没有足够的行动点！");
        }
    }

    // 通过坐标获取对应的 HexTile 说实话，这段代码不应该写在这里的
    private HexTile GetTileAtCoordinates(Coordinates coords)
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            if (tile.coordinates.x == coords.x && tile.coordinates.z == coords.z)
            {
                return tile;
            }
        }
        return null;
    }

    // Handle dragging object movement
    private void HandleDragging()
    {
        Vector3 mousePosition = GridBuildingSystem.GetMousePos();
        selectedObj.transform.position = Snap(mousePosition);

        currentCoords = WorldToCoordinates(selectedObj.transform.position, HexTile.radius);

        // === 直接使用 HexTile 的 isOccupied 判断 ===
        HexTile currentTile = GetTileAtCoordinates(currentCoords);
        Chess movingChess = selectedObj.GetComponent<Chess>();

        if (currentTile != null && currentTile.isOccupied && currentTile.occupyingChess != movingChess)
        {
            selectedObj.transform.position = lastPosition;
            Debug.Log("不能经过被占用的格子！");
            return;
        }

        /** 采用坐标而非碰撞检测的方式去判断棋盘格是否被占据了，可以作为isOccupied的正确参考
        // === 新增：检查拖动经过的格子是否被其他棋子占据 ===
        bool occupiedByOther = false;

        foreach (var other in HexTile.allChess)
        {
            if (other == null) continue;
            if (other == selectedObj.GetComponent<Chess>()) continue;  // 自己不算

            if (other.position.x == currentCoords.x &&
                other.position.z == currentCoords.z)
            {
                occupiedByOther = true;
                break;
            }
        }

        // 如果被占用 → 不能经过，回到 originPosition
        if (occupiedByOther)
        {
            selectedObj.transform.position = originPosition;
            Debug.Log(" 不能经过被占用的格子！");
            return; // 直接终止本帧拖拽逻辑
        }
        **/

        currentPosition = selectedObj.transform.position;

        Debug.Log($"拖拽前物体坐标: ({currentCoords.x}, {currentCoords.z})");
        if (HasMovedALot(coords, currentCoords))
        {
            selectedObj.transform.position = originPosition;
        }
        else if (HasMovedOneStep(coords, currentCoords))
        {
            
            player.actionPoints--;
            coords = currentCoords;
            position = currentPosition;
            //FYJ 加数值
            var chess = selectedObj.GetComponent<Chess>();
            for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
            {
                if (GameManager.Instance.tiles[i].coordinates.x == coords.x && GameManager.Instance.tiles[i].coordinates.z == coords.z)
                {
                    HexTile HexTile = GameManager.Instance.tiles[i];
                    chess.CollectTileValue(HexTile);
                    break;
                }
            }
        }

        UpdatePath(selectedObj.transform.position);
    }

    // 返回到原始位置，并结束拖拽和选中
    private void ReturnToOriginalPosition()
    {
        selectedObj.GetComponent<Chess>().number = originalNumber; // 恢复原始数值
        selectedObj.transform.position = originPosition; // 返回到原始位置
        player.actionPoints = currentAP; // 恢复行动点
        currentState = ObjectState.None; // 结束拖拽
        Unhighlight(); // 取消高亮
        ClearPath(); // 清除路径
    }

    // Reset position to origin
    private void ResetPosition()
    {
        selectedObj.GetComponent<Chess>().number = originalNumber;
        selectedObj.transform.position = originPosition;
        player.actionPoints = currentAP;
        currentState = ObjectState.None;
        Unhighlight();
    }

    // Raycast to detect object clicked
    public GameObject RayToGetObj()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    // 判断是否移动了（根据坐标差）
    private bool HasMovedOneStep(Coordinates start, Coordinates current)
    {
        int distance = (Mathf.Abs(current.x - start.x) + Mathf.Abs(current.z - start.z) + Mathf.Abs((current.x - start.x) + (current.z - start.z))) / 2;
        return distance == 1;
    }

    private bool HasMovedALot(Coordinates start, Coordinates current)
    {
        int distance = (Mathf.Abs(current.x - start.x) + Mathf.Abs(current.z - start.z)+  Mathf.Abs((current.x - start.x) + (current.z - start.z))) / 2;
        return distance > 1;
    }

    // Snap 方法，通过调用 GridBuildingSystem 的 Snap 函数来实现对齐
    private Vector3 Snap(Vector3 rootPos)
    {
        return GridBuildingSystem.Instance.Snap(rootPos);
    }

    // 更新路径（在物体移动时绘制路径）
    private void UpdatePath(Vector3 currentPosition)
    {
        int currentPosCount = lineRenderer.positionCount;
        lineRenderer.positionCount = currentPosCount + 1;
        lineRenderer.SetPosition(currentPosCount, currentPosition);
    }

    // 清除路径（物体放下或取消选中时调用）
    private void ClearPath()
    {
        lineRenderer.positionCount = 0;
    }

    // 高亮物体
    private static void Highlight()
    {
        if (selectedObj != null)
        {
            Outline outline = selectedObj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = true;
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
                outline.enabled = false;
            }
            selectedObj = null;
        }
    }
}

using UnityEngine;
using static GridBuildingSystem;

public class MovingObject : MonoBehaviour
{
    private static GameObject selectedObj; // 当前选中的物体
    private Vector3 offset; // 物体与鼠标的偏移量
    private Vector3 originalPosition; // 存储物体的原始位置
    private bool isDragging = false; // 是否正在拖动物体
    private bool isObjectSelected = false; // 标记物体是否已经选中

    // LineRenderer 用来绘制路径
    private LineRenderer lineRenderer;
    private Vector3 startPosition;

    void Start()
    {
        // 初始化 LineRenderer 组件
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;  // 初始时没有任何点
        lineRenderer.startWidth = 0.1f;  // 设置路径线的宽度
        lineRenderer.endWidth = 0.1f;    // 设置路径线的宽度
    }

    void Update()
    {
        // 如果鼠标左键按下，选中物体
        if (Input.GetMouseButtonDown(0))
        {
            // 执行射线检测，获取点击的物体
            RaycastHit hit = CastRay();
            if (hit.collider == null) { return; }

            GameObject clickedObject = hit.collider.gameObject;

            /*
            // 检查是否有MovableObject脚本
            MovableObject movableObject = clickedObject.GetComponent<MovableObject>();
            if (movableObject == null)
            {
                // 如果没有MovableObject脚本，忽略该物体
                return;
            }

            
            // 检查是否有PlacebleObject脚本
            PlacebleObject placebleObject = clickedObject.GetComponent<PlacebleObject>();
            if (placebleObject == null)
            {
                // 如果没有MovableObject脚本，忽略该物体
                return;
            }
            */

            // 如果当前有物体被选中，取消选中并取消高亮
            if (isObjectSelected && selectedObj != clickedObject)
            {
                Unhighlight();  // 取消当前物体的高亮
                isObjectSelected = false;
            }

            // 如果之前没有选中物体，选中当前物体
            if (!isObjectSelected)
            {
                selectedObj = clickedObject;
                isObjectSelected = true;

                // 高亮表示被选中
                Highlight();

                // 计算选中物体与鼠标之间的偏移
                offset = selectedObj.transform.position - hit.point;

                // 存储物体的原始位置
                originalPosition = selectedObj.transform.position;

                // 记录起始位置
                startPosition = selectedObj.transform.position;

                // 开始绘制路径
                lineRenderer.positionCount = 1; // 从起始位置开始
                lineRenderer.SetPosition(0, startPosition); // 设置第一个位置
                Debug.Log("物体已选中，按 Z 键开始拖动");
            }
            // 如果物体已选中并且已经在拖动，点击时将物体放下
            else if (isDragging)
            {
                // 将物体停在当前的位置
                isDragging = false;
                isObjectSelected = false;
                Unhighlight();
                Debug.Log("物体已放下");

                // 清除路径（物体放下后，隐藏路径）
                ClearPath();
            }
        }

        // 右键点击取消高亮并取消选中物体
        if (Input.GetMouseButtonDown(1))
        {
            // 如果物体正在拖动，右键点击时返回原始位置并取消拖动
            if (isDragging)
            {
                // 将物体返回到原始位置
                if (selectedObj != null)
                {
                    selectedObj.transform.position = originalPosition;
                }
                Unhighlight();
                isDragging = false;
                isObjectSelected = false;
                Debug.Log("物体已返回原位置并取消拖动");

                // 清除路径（物体返回原位置后，隐藏路径）
                ClearPath();
            }
            else if (isObjectSelected)
            {
                // 如果物体没有在拖动状态下右键点击，取消高亮
                Unhighlight();
                isObjectSelected = false;
                selectedObj = null;
                Debug.Log("物体已取消选中");

                // 清除路径（取消选中物体后，隐藏路径）
                ClearPath();
            }
        }

        // 按下 Z 键开始拖动或返回原位置
        if (isObjectSelected && Input.GetKeyDown(KeyCode.Z))
        {
            // 如果没有拖动，开始拖动
            isDragging = true;
            Debug.Log("开始拖动物体");
        }

        // 如果正在拖动物体
        if (isDragging && selectedObj != null)
        {
            // 获取鼠标位置并将物体拖动到该位置，同时使用 Snap 函数来对齐到网格
            Vector3 mousePosition = GridBuildingSystem.GetMousePos();
            selectedObj.transform.position = Snap(mousePosition); // 使用 Snap 函数对齐到网格

            // 更新路径线
            UpdatePath(selectedObj.transform.position);
        }
    }

    // 执行射线检测
    private RaycastHit CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从鼠标位置发射射线

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) // 检测射线是否击中物体
        {
            return hit;
        }

        return default;
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

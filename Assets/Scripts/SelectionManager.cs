using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static GameObject selectedObj;  // 当前选中的物体
    public static bool isObjectSelected = false;  // 标记物体是否已选中
    public static bool isAttackMode; // 标记物体是否在攻击模式
    public float maxDistance = 100f; // 射线最大距离

    // 使用List存储射线命中结果
    private List<RaycastHit> hitList = new List<RaycastHit>();

    void Update()
    {
        // 如果鼠标左键按下，选中物体
        if (Input.GetMouseButtonDown(0) && isAttackMode==false)
        {
            // 执行射线检测，获取点击的物体
            GameObject clickedObject = RayToGetObj();
            if (clickedObject == null) return;

            // 检查是否有PlacebleObject脚本
            PlacebleObject placebleObject = clickedObject.GetComponent<PlacebleObject>();
            if (placebleObject == null) return;

            // === 阵营检测逻辑 ===
            King clickedKing = clickedObject.GetComponent<King>();
            if (clickedKing != null)
            {
                // 获取当前回合玩家（来自 TurnManager）
                TurnManager turnManager = FindObjectOfType<TurnManager>();
                Player currentPlayer = turnManager.players[turnManager.turnCount];
                Player clickedPlayer = clickedKing.player;

                if (clickedPlayer != currentPlayer)
                {
                    Debug.Log($"当前回合属于 {currentPlayer.playerName}，无法选中 {clickedPlayer.playerName} 的棋子！");
                    if (UnitInfoPanelController.Instance != null)
                        UnitInfoPanelController.Instance.ShowInvalidAction("Not your turn!");
                    return;
                }
            }

            SelectObject(clickedObject);
            Debug.Log("选中物体");
        }
        // 如果鼠标左键按下，选中物体
        if (Input.GetMouseButtonDown(0) && isAttackMode == true)
        {
            Attack_RayToGetObj();
        }
        // 右键点击取消高亮并取消选中物体
        if (Input.GetMouseButtonDown(1))
        {
            if (isObjectSelected)
            {
                DeselectObject();
            }
        }
    }

    // 选中物体并高亮显示
    public static void SelectObject(GameObject obj)
    {
        if (isObjectSelected && selectedObj != obj)
        {
            Unhighlight();
        }

        selectedObj = obj;
        isObjectSelected = true;

        Highlight();
    }

    // 取消选中物体
    public static void DeselectObject()
    {
        if (isObjectSelected)
        {
            Unhighlight();
            isObjectSelected = false;
            selectedObj = null;
        }
    }

    // 获得选中的物体
    public static GameObject getSelectedObject()
    {
        return selectedObj;
    }

    // 高亮选中的物体
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

    // 取消高亮显示
    private static void Unhighlight()
    {
        if (selectedObj != null)
        {
            Outline outline = selectedObj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
    }

    // 执行射线检测
    public static GameObject RayToGetObj()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 从鼠标位置发射射线
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // 如果射线击中了物体
        {
            //Debug.Log("射线命中物体名字：" + hit.collider.gameObject);
            return hit.collider.gameObject; // 返回被点击的物体
        }

        return null; // 如果没有击中物体，返回 null
    }


    public void Attack_RayToGetObj()
    {
        hitList.Clear();
        bool Is_CanAttack = false;
        GameObject Enemy=null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 获取所有命中结果
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);

        // 将数组转换为List
        hitList.AddRange(hits);

        // 遍历所有击中的物体
        foreach (RaycastHit hit in hitList)
        {
            if(hit.collider.gameObject== selectedObj)
            {
                hitList.Remove(hit);
                return;
            }
            //Debug.Log($"击中物体: {hit.collider.name}，距离: {hit.distance}");
            if(hit.collider.GetComponent<HexTile>()!=null)
            {
            if(hit.collider.GetComponent<Renderer>().material.color==hit.collider.GetComponent<HexTile>().highlighColor)
            {
                    Is_CanAttack = true;
            }
            }
            if(hit.collider.GetComponent<King>()!=null)
            {
                Enemy = hit.collider.gameObject;
            }
          
            // 在这里处理每个击中的物体
            // 例如：hit.collider.GetComponent<YourComponent>()
        }
        if(Is_CanAttack==true&&Enemy!=null)
        {
            Enemy.GetComponent<King>().TakeDamage();
        }
    }
}

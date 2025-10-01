using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HexMath;
using static MovingObject;
public class Chess : MonoBehaviour
{
    public int number = 1;
    public Player player;
    private string name;
    private string className;
    private int id;
    private int attackArea = 2;
    public Coordinates position;
    public HexTile[] tiles;
    public GameManager gameManager;
    public bool isInAttackMode = false;
    [Header("UI属性面板")]
    public UnitInfoPanelController panel;
    MovingObject move;
    public Renderer rend;
    public void Init(string className, int id, Player owner)
    {
        gameManager = GetComponent<GameManager>();
        var go = GameObject.Find("GameManager");
        if (go != null) move = go.GetComponent<MovingObject>();
        this.className = className;
        this.id = id;
        this.player = owner;
        this.name = className + "_" + id;
        gameObject.name = this.name;
    }
    // Start is called before the first frame update
    void Start()
    {
        // 自动获取场景中的 UI 面板
        if (panel == null)
        {
            panel = Resources.FindObjectsOfTypeAll<UnitInfoPanelController>().FirstOrDefault(); ;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //选中逻辑
        if (SelectionManager.selectedObj == null) return;
        if (SelectionManager.selectedObj != gameObject) return;
        //UI关闭
        if (panel != null && Input.GetMouseButtonDown(1)) panel.Hide();
        if (Input.GetKeyDown(KeyCode.X) && move.currentState == ObjectState.Selected)
        {
            isInAttackMode = !isInAttackMode;
            if (isInAttackMode)
                showAttackableTiles();
            else
                ResetTiles();
        }

        
        if (isInAttackMode && Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = GridBuildingSystem.GetMousePos(); // 获取鼠标点到地面上的世界坐标
            Coordinates targetCoord = HexMath.WorldToCoordinates(mouseWorldPos, radius: 1f); // radius 是格子边长，和你地图生成用的保持一致

            // 遍历场上所有棋子，找到目标棋子
            foreach (Chess potentialTarget in FindObjectsOfType<Chess>())
            {
                if (potentialTarget == this) continue; // 不攻击自己
                if (potentialTarget.player == this.player) continue; // 不攻击友军

                if (potentialTarget.position.x == targetCoord.x && potentialTarget.position.z == targetCoord.z)
                {
                    ExecuteAttack(potentialTarget);
                    ResetTiles();
                    isInAttackMode = false;
                    break;
                }
            }
        }
        
    }

    void OnMouseDown()//UI显示
    {
        SelectionManager.selectedObj = gameObject;
        if (panel != null)
        {
            panel.ShowUnit(gameObject.name, number);
        }
    }
    public void showAttackableTiles()//显示可攻击范围
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            int distX = tile.coordinates.x - position.x;
            int distZ = tile.coordinates.z - position.z;
            int dist = distX + distZ;
            if ((Mathf.Abs(distX) + Mathf.Abs(distZ) + Mathf.Abs(dist)) / 2 <= attackArea)
            {
                tile.HighlightTile();
            }
        }
    }
    public void ResetTiles()//攻击范围颜色显示
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            tile.ResetTile();
        }
    }

    
    void ExecuteAttack(Chess target)
    {
        int aBefore = this.number;
        int bBefore = target.number;

        this.number -= bBefore;
        target.number -= aBefore;

        Debug.Log($"{name} 攻击 {target.name}：我方减 {bBefore}，敌方减 {aBefore}");

        if (panel != null) panel.ShowUnit(gameObject.name, number); // 更新自己面板

        if (this.number <= 0)
        {
            Destroy(gameObject);
            Debug.Log($"{name} 被击败！");
        }

        if (target.number <= 0)
        {
            Destroy(target.gameObject);
            Debug.Log($"{target.name} 被击败！");
        }
    }
    

    public void CollectTileValue(HexTile tile)//收集格子值
    {
        int value = tile.GetTileValue();
        number += value;
        Debug.Log($"{name} ���� {tile.name}����� {value} �㣬��ǰֵΪ��{number}");
    }
}

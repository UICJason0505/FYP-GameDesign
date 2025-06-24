using System.Collections;
using System.Collections.Generic;
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


    public void Init(string className, int id, Player owner)
    {
        gameManager = GetComponent<GameManager>();
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
            panel = FindObjectOfType<UnitInfoPanelController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //选中逻辑
        if (SelectionManager.selectedObj == null) return;
        //UI关闭
        if (panel != null && Input.GetMouseButtonDown(1))
        {
            panel.Hide();
        }
        //攻击逻辑
        if (Input.GetKeyDown(KeyCode.X) && isInAttackMode == false)
        {
            showAttackableTiles();
            isInAttackMode = true;
            return;
        }
        if (Input.GetKeyDown(KeyCode.X) && isInAttackMode == true)
        {
            ResetTiles();
            isInAttackMode = false;
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
    public void showAttackableTiles()
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
    public void ResetTiles()
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            tile.ResetTile();
        }
    }
    public void CollectTileValue(HexTile tile)
    {
        int value = tile.GetTileValue();
        number += value;
        Debug.Log($"{name} ���� {tile.name}����� {value} �㣬��ǰֵΪ��{number}");
    }
}

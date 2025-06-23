using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HexMath;
using static MovingObject;
public class Chess : MonoBehaviour
{
    public int number = 0;
    public Player player;
    private string name;
    private string className;
    private int id;
    public int attackArea = 1;
    public Coordinates position;
    public HexTile[] tiles;
    public GameManager gameManager;
    public bool isInAttackMode = false;
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
        
    }
    // Update is called once per frame
    void Update()
    {
        if (MovingObject.selectedObj == null) return;
        if (Input.GetKeyDown(KeyCode.X)&& MovingObject.selectedObj == this && isInAttackMode == false)
        {
            showAttackableTiles();
            isInAttackMode = true;
        }
        if (Input.GetKeyDown(KeyCode.X) && MovingObject.selectedObj == this && isInAttackMode == true)
        {
            showAttackableTiles();
            isInAttackMode = false;
        }
    }
    public void showAttackableTiles()
    {
        for (int i = 0; i < GameManager.Instance.tiles.Length; i++)
        {
            HexTile tile = GameManager.Instance.tiles[i];
            int distX = Mathf.Abs(tile.coordinates.x - position.x);
            int distZ = Mathf.Abs(tile.coordinates.z - position.z);
            if (distX + distZ <= attackArea)
            {
                tile.HighlightTile();
            }
        }
    }
}

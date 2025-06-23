using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HexMath;

public class Chess : MonoBehaviour
{
    public int number = 0;
    public Player player;
    private string name;
    private string className;
    private int id;
    public int attackArea = 1;
    public Coordinates position;
    public void Init(string className, int id, Player owner)
    {
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
        
    }
    public void showAttackableTiles()
    {

    }
}

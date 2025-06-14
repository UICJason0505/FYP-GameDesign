using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNum = 2;
    public int playerID;         
    public string playerName;    
    public bool canOperate = false;

    public Player(int id, string name)
    {
        this.playerID = id;
        this.playerName = name;
        canOperate = false;
    }
}

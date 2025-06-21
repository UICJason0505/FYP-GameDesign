using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int playerNum = 2;
    public int playerID;
    public string playerName;
    public bool canOperate = false;
    public int actionPoints = 10;
    public Player(int id, string name)
    {
        this.playerID = id;
        this.playerName = name;
        canOperate = false;
        actionPoints = 10; // Default action points
    }
    public void ResetActionPoints()
    {
        actionPoints = 10;
    }

    public bool HasActionPoints()
    {
        return actionPoints > 0;
    }

    public void UseActionPoint()
    {
        if (actionPoints > 0)
            actionPoints--;
    }
}

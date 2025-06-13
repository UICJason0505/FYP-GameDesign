using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerNum = 2;
    public int playerId;         
    public string playerName;    
    public bool canOperate = false;    

    public void PlayerData(int id)
    {
        playerId = id;
        canOperate = false;
    }
}

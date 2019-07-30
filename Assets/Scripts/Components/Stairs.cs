using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stairs : MonoBehaviour
{
    public bool hasBeenSeen = false;
    //public Engine engine;
    //UIManager uiManager = null;

    //saving them not working???....

    void Start()
    {
        //engine = FindObjectOfType<Engine>();
        //uiManager = FindObjectOfType<UIManager>();
        FindObjectOfType<UIManager>().DungeonLevelUpdate(FindObjectOfType<Engine>().dungeonLevel);
    }

    public void UseStairs()
    {
        FindObjectOfType<Engine>().dungeonLevel++;
        FindObjectOfType<UIManager>().DungeonLevelUpdate(FindObjectOfType<Engine>().dungeonLevel);
        GameMap gameMap = FindObjectOfType<GameMap>();
        gameMap.NewMap();
        gameMap.UpdateDicts();
    }

}

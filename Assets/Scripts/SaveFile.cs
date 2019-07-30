using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public static SaveFile current;
    //save data for player
    public PlayerInfo playerInfo;

    //data for map
    public MapInfo mapInfo;

    public ObjectInfo objectInfo;


    public SaveFile(PlayerInfo PlayerInfo, MapInfo MapInfo, ObjectInfo ObjectInfo)
    {
        playerInfo = PlayerInfo;
        mapInfo = MapInfo;
        objectInfo = ObjectInfo;
    }
}

[System.Serializable]
public class PlayerInfo
{
    public int maxhp;
    public int hp;
    public int defense;
    public int power;
    public List<Item> inventoryItems = new List<Item>();

    public PlayerInfo(int MaxHP, int HP, int Defense, int Power, List<Item> InventoryItems)
    {
        maxhp = MaxHP;
        hp = HP;
        defense = Defense;
        power = Power;
        inventoryItems = InventoryItems;
    }
}

[System.Serializable]
public class MapInfo
{
    Cell[,] grid;

    public bool[,] tileSeen = new bool[76, 40];
    public bool[,] tileVisible = new bool[76, 40];
    public bool[,] tileIsWall = new bool[76, 40];
    public bool[,] hasEntity = new bool[76, 40];
    public bool[,] hasItem = new bool[76, 40];
    public bool[,] isWalkable = new bool[76, 40];

    public MapInfo(Cell[,] Grid, bool[,] TileSeen, bool[,] TileVisible, bool[,] TileIsWall, bool[,] HasEntity, bool[,] HasItem, bool[,] IsWalkable)
    {
        grid = Grid;
        tileSeen = TileSeen;
        tileVisible = TileVisible;
        tileIsWall = TileIsWall;
        hasEntity = HasEntity;
        hasItem = HasItem;
        isWalkable = IsWalkable;
    }
}

[System.Serializable]
public class ObjectInfo
{
    public List<GameObject> entities = new List<GameObject>();
    public List<GameObject> deadEntities = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> equipment = new List<GameObject>();

    public ObjectInfo(List<GameObject> Entities, List<GameObject> DeadEntities, List<GameObject> Items, List<GameObject> Equipment)
    {
        entities = Entities;
        deadEntities = DeadEntities;
        items = Items;
        equipment = Equipment;
    }
}

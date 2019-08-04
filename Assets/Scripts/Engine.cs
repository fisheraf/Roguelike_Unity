using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using BayatGames.SaveGameFree;

public class Engine : MonoBehaviour
{
    [SerializeField] GameObject entityObject;
    [SerializeField] GameObject playerObject;
    public GameObject itemObject;
    public GameObject equipmentObject;
    [SerializeField] GameObject stairsObject = null;
    [SerializeField] UIManager uiManager;
    [SerializeField] Inventory inventory;
    Entity entity;
    Entity playerEntity;
    //[SerializeField] GameObject player;
    [SerializeField] Tilemap tilemap;
    GameMap gameMap;
    Grid2 grid2;

    public GameObject[] enemy;

    public Vector3 worldPoint;
    public Vector2Int target;
    GameObject highlight = null;

    public List<GameObject> entities;
    public List<GameObject> deadEntities;
    public List<GameObject> items;

    GameObject stairsObj = null;
    public int dungeonLevel = 0;

    public enum GameState { PlayerTurn, EnemyTurn, PlayerDead, ShowInventory, DropInventory, Targeting, LevelUp }

    public GameState gameState = GameState.PlayerTurn;
    GameState lastGameState;

    public Stopwatch stopwatch;

    // Start is called before the first frame update
    void Awake()
    {
        gameMap = FindObjectOfType<GameMap>().GetComponent<GameMap>();
        grid2 = FindObjectOfType<Grid2>();
        tilemap = FindObjectOfType<Tilemap>();
        uiManager = FindObjectOfType<UIManager>();
        inventory = GetComponent<Inventory>();

        highlight = GameObject.Find("Highlight");
        highlight.GetComponentInChildren<SpriteRenderer>().enabled = false;

        entities = gameMap.entities;
        deadEntities = gameMap.deadEntities;
        items = gameMap.items;
        //player = GameObject.Find("Player");
        //entity = player.GetComponent<Entity>();

        //gameState = GameState.PlayerTurn;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            //SaveProgress();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            //LoadProgress();
        }


        if (gameState == GameState.PlayerTurn)
        {
            Movement();
            Actions();
            DropInventory();
        }

        //enemy turn test
        if (gameState == GameState.EnemyTurn)
        {  
            EnemyTurn();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameState = GameState.PlayerTurn;
            }
        }

        if(gameState == GameState.PlayerTurn || gameState == GameState.PlayerDead)
        {
            ShowInventory();
        }

        if(gameState == GameState.ShowInventory || gameState == GameState.DropInventory)
        {
            InventoryInput();
        }

        if (gameState == GameState.Targeting)
        {
            highlight.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
        else
        {
            highlight.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        MouseOverTooltips();
    }

    private void MouseOverTooltips()
    {
        worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool mouseIsOverSomething = false;

        //foreach player, item, dead entity other object that dont have entity
        foreach (GameObject entity in entities)
        {
            if (entity.transform.position.x == (int)worldPoint.x && entity.transform.position.y == (int)worldPoint.y)
            {
                if(gameMap.tileVisible[(int)worldPoint.x, (int)worldPoint.y])
                {
                    uiManager.MouseOverText(entity.GetComponent<Entity>().name);
                    mouseIsOverSomething = true;

                    //flavor text?
                }
            }
        }
        foreach (GameObject item in items)
        {
            if (item.transform.position.x == (int)worldPoint.x && item.transform.position.y == (int)worldPoint.y)
            {
                if (gameMap.tileVisible[(int)worldPoint.x, (int)worldPoint.y])
                {
                    uiManager.MouseOverText(item.GetComponent<Item>().name);
                    mouseIsOverSomething = true;

                    //flavor text?
                }
            }
        }
        foreach (GameObject deadEntity in deadEntities)
        {
            if (deadEntity.transform.position.x == (int)worldPoint.x && deadEntity.transform.position.y == (int)worldPoint.y)
            {
                if (gameMap.tileVisible[(int)worldPoint.x, (int)worldPoint.y])
                {
                    uiManager.MouseOverText(deadEntity.GetComponent<Entity>().name);
                    mouseIsOverSomething = true;

                    //flavor text?
                }
            }
        }

        if(!mouseIsOverSomething)
        {
            uiManager.MouseOverText(null);
        }
    }


    /*
    public void CreateEntity(int x, int y, int z, int spriteNumber, Color32 color, string name, int HP, int defense, int power)
    {
        entityObject = Instantiate(entityObject, new Vector3(x, y, z), Quaternion.identity);
        entity = entityObject.GetComponent<Entity>();
        SpriteRenderer spriteRenderer = entity.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = entity.entitySprites[spriteNumber];
        spriteRenderer.color = color;
        entityObject.name = name;

        Fighter fighter = entity.GetComponent<Fighter>();
        fighter.MaxHP = HP;
        fighter.HP = HP;
        fighter.defense = defense;
        fighter.power = power;


        gameMap.entities.Add(entityObject);

        /*
        if (fighter)
        {
            entityObject.AddComponent<Fighter>();
        }
        if (ai)
        {
            entityObject.AddComponent<AI>();
        }
        *

        //entity.entitySprite =  //list of sprites?
    }*/

    public void CreateEntity(int x, int y, int entityNumber)
    {
        entityObject = Instantiate(enemy[entityNumber], new Vector3(x, y, -1), Quaternion.identity);
        entityObject.name = enemy[entityNumber].name;
        entity = entityObject.GetComponent<Entity>();
        entity.entityNumber = entityNumber;

        gameMap.entities.Add(entityObject);
    }
        

    public void CreatePlayer(int x, int y, int z, int HP, int defense, int power)
    {
        playerObject = Instantiate(playerObject, new Vector3(x,y,z), Quaternion.identity);
        playerObject.name = "Player";

        Fighter fighter = playerObject.GetComponent<Fighter>();
        fighter.MaxHP = HP;
        fighter.HP = HP;
        fighter.baseDefense = defense;
        fighter.basePower = power;

        playerEntity = GameObject.Find("Player").GetComponent<Entity>();
        uiManager.SetPlayer();
        FindObjectOfType<LevelUp>().SetPlayer();
    }

    /*
    public void CreateItem(int x, int y, int z, Item.ItemType itemType, int value)//enum?
    {
        itemObject = Instantiate(itemObject, new Vector3(x, y, z), Quaternion.identity);
        //itemObject.name = name;
        itemObject.GetComponent<Item>().SetItem(itemType, value);

        gameMap.items.Add(itemObject);
    }
    */

    public void CreateItem(int x, int y, int itemNumber)
    {
        itemObject = Instantiate(itemObject, new Vector3(x, y, -1), Quaternion.identity);
        itemObject.GetComponent<Item>().SetItem(itemNumber);
        itemObject.GetComponentInChildren<SpriteRenderer>().enabled = false;

        gameMap.items.Add(itemObject);
    }

    public void CreateEquipment(int x, int y, int eNumber)
    {
        //Debug.Log("create" + eNumber);
        equipmentObject = Instantiate(equipmentObject, new Vector3(x, y, -1), Quaternion.identity);
        equipmentObject.GetComponent<Equippable>().SetEquipment(eNumber);
        equipmentObject.GetComponentInChildren<SpriteRenderer>().enabled = false;

        gameMap.items.Add(equipmentObject);
    }

    public void CreateStairs(Vector2 lastRoom)
    {
        if(stairsObj == null)
        {
            stairsObj = Instantiate(stairsObject, new Vector3((int)lastRoom.x, (int)lastRoom.y, -1), Quaternion.identity);
            stairsObj.name = "Stairs";
            gameMap.stairs = stairsObj;
            stairsObj.GetComponentInChildren<SpriteRenderer>().enabled = false;
            stairsObj.GetComponent<Stairs>().hasBeenSeen = false;
        }
        else
        {
            stairsObj.transform.position = new Vector3((int)lastRoom.x, (int)lastRoom.y, -1);
            stairsObj.GetComponentInChildren<SpriteRenderer>().enabled = false;
            stairsObj.GetComponent<Stairs>().hasBeenSeen = false;
        }
    }


    private void Movement()
    {
        //own script?
        //Entity.Move(int dx, int dy)
        //down left
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Move(-1, -1);
            //gameState = GameState.EnemyTurn;
        }
        //down
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Move(0, -1);
            //gameState = GameState.EnemyTurn;
        }
        //down right
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Move(1, -1);
            //gameState = GameState.EnemyTurn;
        }
        //left
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Move(-1, 0);
            //gameState = GameState.EnemyTurn;
        }
        //wait
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {            
            gameState = GameState.EnemyTurn;
        } 
        //right
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Move(1, 0);
            //gameState = GameState.EnemyTurn;
        }
        //up left
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Move(-1, 1);
            //gameState = GameState.EnemyTurn;
        }
        //up
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            Move(0, 1);
            //gameState = GameState.EnemyTurn;
        }
        //up right
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            Move(1, 1);
            //gameState = GameState.EnemyTurn;
        }

    }

    void Move(int dx, int dy)
    {
        Vector3Int targetTile = new Vector3Int(Mathf.RoundToInt(playerEntity.transform.position.x + dx),
                    Mathf.RoundToInt(playerEntity.transform.position.y + dy), 0);

        //List<GameObject> entities = gameMapObj.entities;
        foreach (GameObject entity in entities)
        {
            if(entity.transform.position.x == targetTile.x && entity.transform.position.y == targetTile.y)
            {
                //Debug.Log("You tickle the " + entity.name);// + " in the shin");
                playerObject.GetComponent<Fighter>().attack(entity.GetComponent<Fighter>());
                if(gameState == GameState.PlayerTurn)
                {
                    gameState = GameState.EnemyTurn;
                }
                return;
            }            
        }
        
        if (tilemap.GetTile(targetTile).name == "WallTile")
        {
            Debug.Log("wall");
        }
        else
        {
            playerEntity.Move(dx, dy);
            gameState = GameState.EnemyTurn;

            //gameMapObj.FOVRecompute();
            gameMap.FOV();
            //gameMapObj.GetVisibleCells();
        }        
    }

    void Actions()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            PickUpItem();
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            UseStairs();
        }
    }

    void PickUpItem()
    {
        bool itemPickedUp = false;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].transform.position.x == playerEntity.transform.position.x && items[i].transform.position.y == playerEntity.transform.position.y)
            {
                items[i].GetComponent<Item>().PickUpObject();
                itemPickedUp = true;
                gameState = GameState.EnemyTurn;
            }
        }
        /*
        foreach (GameObject item in items)
        {            
            if (item.transform.position.x == playerEntity.transform.position.x && item.transform.position.y == playerEntity.transform.position.y)
            {
                item.GetComponent<Item>().PickUpObject();
                itemPickedUp = true;
                gameState = GameState.EnemyTurn;
            }
        }
        */
        if(itemPickedUp == false)
        {
            uiManager.NewMessage("There is nothing to pickup.");
        }
    }

    void UseStairs()
    {
        Debug.Log("enter");
        if (stairsObj.transform.position.x == playerEntity.transform.position.x && stairsObj.transform.position.y == playerEntity.transform.position.y)
        {
            Debug.Log("stairs");
            stairsObject.GetComponent<Stairs>().UseStairs();
        }
    }


    float timer;

    void ShowInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            lastGameState = gameState;
            gameState = GameState.ShowInventory;
            inventory.OpenInventory();

            timer = .3f;
        }
    }

    void DropInventory()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            lastGameState = gameState;
            gameState = GameState.DropInventory;
            inventory.OpenInventory();

            timer = .3f;
        }
    }

    void InventoryInput()
    {        
        if(Input.GetKeyDown(KeyCode.Escape))
        { 
            inventory.CloseInventory();
            gameState = lastGameState;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.D))
             {
                inventory.CloseInventory();
                gameState = lastGameState;
            }
        }    
    }

    public bool targetSelected = false;
    
    public IEnumerator TargetSelect(bool entityRequired)
    {
        targetSelected = false;
        inventory.CloseInventory();

        while(targetSelected == false)
        {
            gameState = GameState.Targeting;

            //highlight target cell
            highlight.transform.position = new Vector3((int)worldPoint.x, (int)worldPoint.y, -1);
            highlight.GetComponentInChildren<SpriteRenderer>().enabled = true;


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameState = GameState.ShowInventory;
                inventory.OpenInventory();
                yield break;
            }

            else if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                target = new Vector2Int((int)worldPoint.x, (int)worldPoint.y);                               

                if (gameMap.tileVisible[target.x, target.y])
                {
                    if(entityRequired)
                    {
                        foreach(GameObject entity in entities)
                        {
                            if((int)entity.transform.position.x == target.x && (int)entity.transform.position.y == target.y)
                            {
                                targetSelected = true;
                            }
                        }
                        yield return null;
                    }
                    else
                    {
                        targetSelected = true;
                    }
                }
                else
                {
                    uiManager.NewMessage("Target not in range.");
                    yield return null;
                }
                inventory.CloseInventory();
            }
            yield return null;
        }
        //item.Fireball(target);
    }
    

    public void AttemptMove(Unit unit, Vector3 target)
    {
        //place entity in center of new cell
        //target = new Vector3(target.x - .5f, target.y - .5f, target.z);
        /*
        foreach (GameObject entity in entities)
        {
            if (entity.transform.position.x == target.x && entity.transform.position.y == target.y)
            {
                Debug.Log(entity.name + " kicks " + playerEntity.name + " in the shin");
                entity.GetComponent<Fighter>().attack(playerObject.GetComponent<Fighter>());
                gameState = GameState.EnemyTurn;
                return;
            }
        }
        */
        //StartCoroutine(MoveToNextSpot(unit, target));
        MoveToNextSpot2(unit, target);

    }

    IEnumerator MoveToNextSpot(Unit unit, Vector3 target)
    {
        while(unit.transform.position != target)
        {
            

            unit.transform.position = Vector3.MoveTowards(unit.transform.position, target, unit.speed * Time.deltaTime);
            yield return null;


            //entity.Move(dx, dy);
            if(unit.transform.position == target)
            {
                //gameState = GameState.PlayerTurn;
                yield break;
            }
        }
    }

    void MoveToNextSpot2(Unit unit, Vector3 target)
    {
        gameMap.hasEntity[(int)unit.transform.position.x, (int)unit.transform.position.y] = false;
        unit.transform.position = target;
        gameMap.hasEntity[(int)unit.transform.position.x, (int)unit.transform.position.y] = true;
        grid2.CreateGrid();
    }




    void EnemyTurn()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (GameObject entity in entities)
        {
            entity.GetComponent<Entity>().hasActed = false;
        }

        foreach (GameObject entity in entities)
        {
            if (entity.GetComponent<Entity>().isDead) { continue; }

            if (entity.name != "Player")
            {
                //Debug.Log("The " + entity + " ponders the meaning of life.");

                //if entity can see player
                //finishing walking to last seen spot?
                if (entity.GetComponent<Entity>().isConfused)
                {
                    entity.GetComponent<Entity>().confusedTurnsLeft--;
                    if(entity.GetComponent<Entity>().confusedTurnsLeft < 0)
                    {
                        uiManager.NewMessage("The " + entity.name + " is no longer confused");
                        entity.GetComponent<Entity>().isConfused = false;
                        entity.GetComponent<Entity>().hasActed = true;
                    }

                    List<Vector2> rTarget = new List<Vector2>();
                    rTarget.Clear();

                    for (int x = (int)entity.transform.position.x - 1; x < (int)entity.transform.position.x + 1; x++)
                    {
                        for (int y = (int)entity.transform.position.y - 1; y < (int)entity.transform.position.y + 1; y++)
                        {

                            if (gameMap.isWalkable[x,y])
                            {
                                rTarget.Add(new Vector2(x, y));
                            }
                        }
                    }
                    rTarget.Add(new Vector2((int)entity.transform.position.x, (int)entity.transform.position.y));
                    int randomNumber = UnityEngine.Random.Range(0, rTarget.Count);
                    
                    Vector3 randomTarget = rTarget[randomNumber];
                    MoveToNextSpot2(entity.GetComponent<Unit>(), randomTarget);
                    Debug.Log(randomTarget);
                    entity.GetComponent<Entity>().hasActed = true;
                }

                if (gameMap.tileVisible[(int)entity.transform.position.x, (int)entity.transform.position.y])
                {
                    bool playerInRange = false;
                    if (entity.GetComponent<Unit>() != null)
                    {
                        List<Node> neighbors = grid2.GetNeighbours(new Node(true, entity.transform.position, (int)entity.transform.position.x, (int)entity.transform.position.y));
                        foreach (Node neighbor in neighbors)
                        {
                            //Debug.Log("Player: " + playerObject.transform.position);
                            //Debug.Log("Neighbor: " + neighbor.worldPosition);
                            if(playerObject.transform.position == neighbor.worldPosition)
                            {
                                playerInRange = true;
                            }                            
                        }

                        if (playerInRange && !entity.GetComponent<Entity>().hasActed)
                        {
                            //Debug.Log(entity.name + " attacks player");
                            entity.GetComponent<Fighter>().attack(playerObject.GetComponent<Fighter>());
                            entity.GetComponent<Entity>().hasActed = true;
                            //gameState = GameState.PlayerTurn;
                        }
                        else if (!playerInRange && !entity.GetComponent<Entity>().hasActed)
                        {
                            entity.GetComponent<Unit>().RequestPathForUnit();
                            entity.GetComponent<Entity>().hasActed = true;
                        }
                        else
                        {
                            gameState = GameState.PlayerTurn;
                        }
                    }
                }
                //else wander?
                //grid2.CreateGrid();
            }
        }
        gameState = GameState.PlayerTurn;
        stopwatch.Stop();
        Debug.Log("Enemy turn took:" + stopwatch.Elapsed);
    }



    bool[] tileSeenSingle = new bool[3115];
    bool[] tileVisibleSingle = new bool[3115];
    bool[] tileIsWallSingle = new bool[3115];
    bool[] hasEntitySingle = new bool[3115];
    bool[] hasItemSingle = new bool[3115];
    bool[] isWalkableSingle = new bool[3115];

    public void SaveProgress()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();

        Debug.Log("Game Saved");
        //player
        SaveGame.Save<Vector3>("Player Location", playerObject.transform.position);

        Fighter playerFighter = playerObject.GetComponent<Fighter>();
        SaveGame.Save<int>("maxHP", playerFighter.MaxHP);
        SaveGame.Save<int>("hp", playerFighter.HP);
        SaveGame.Save<int>("defense", playerFighter.baseDefense);
        SaveGame.Save<int>("power", playerFighter.basePower);

        Level playerLevel = playerObject.GetComponent<Level>();
        SaveGame.Save<int>("currentLevel", playerLevel.currentLevel);
        SaveGame.Save<int>("currentXP", playerLevel.currentXP);

        SaveGame.Save<int>("dungeonLevel", dungeonLevel);

        //SaveGame.Save<List<GameObject>>("Player Items", inventory.items);
        inventory.SaveItems();
        SaveGame.Save<List<int>>("Player Items", inventory.itemIDNumber);


        Equipment equipment = FindObjectOfType<Equipment>();
        equipment.SaveEquipped();
        SaveGame.Save<List<int>>("Player Eq", equipment.eqIDNumber);
        SaveGame.Save<List<bool>>("isEquippedList", equipment.isEquippedList);



        //map
        int i = 0;

        for (int x = 0; x < gameMap.mapWidth; x++)
        {
            for (int y = 0; y < gameMap.mapHeight; y++)
            {
                //Debug.Log(i + "," + x + "," + y);
                tileSeenSingle[i] = gameMap.tileSeen[x, y];
                tileVisibleSingle[i] = gameMap.tileVisible[x, y];
                tileIsWallSingle[i] = gameMap.tileIsWall[x, y];
                hasEntitySingle[i] = gameMap.hasEntity[x, y];
                hasItemSingle[i] = gameMap.hasItem[x, y];
                isWalkableSingle[i] = gameMap.isWalkable[x, y];

                i++;
            }
            //i++;
        }


        SaveGame.Save<bool[]>("tileSeen", tileSeenSingle);
        SaveGame.Save<bool[]>("tileVisible", tileVisibleSingle);
        SaveGame.Save<bool[]>("tileIsWall", tileIsWallSingle);
        SaveGame.Save<bool[]>("hasEntity", hasEntitySingle);
        SaveGame.Save<bool[]>("hasItem", hasItemSingle);
        SaveGame.Save<bool[]>("isWalkable", isWalkableSingle);


        //objects
        //CreateEntity(int x, int y, int z, int spriteNumber, Color32 color, string name, int HP, int defense, int power)

        List<int> xpos = new List<int>();
        List<int> ypos = new List<int>();
        List<int> entityNumber = new List<int>();
        List<int> hpe = new List<int>();

        foreach (GameObject entity in entities)
        {
            xpos.Add((int)entity.transform.position.x);
            ypos.Add((int)entity.transform.position.y);
            entityNumber.Add(entity.GetComponent<Entity>().entityNumber);
            hpe.Add(entity.GetComponent<Fighter>().HP);
        }

        SaveGame.Save<List<int>>("xpos", xpos);
        SaveGame.Save<List<int>>("ypos", ypos);
        SaveGame.Save<List<int>>("entityNumber", entityNumber);
        SaveGame.Save<List<int>>("hpe", hpe);
        //SaveGame.Save<List<GameObject>>("equipment", equipment);

        //deadentities


        //items
        List<int> xposi = new List<int>();
        List<int> yposi = new List<int>();
        List<int> itemIDNumber = new List<int>();

        itemIDNumber.Clear();
        for (int j = 0; j < items.Count; j++)
        {
            if (items[j].name == "Healing Potion")
            {
                itemIDNumber.Add(1);
            }
            if (items[j].name == "Lightning Scroll")
            {
                itemIDNumber.Add(2);
            }
            if (items[j].name == "Fireball Scroll")
            {
                itemIDNumber.Add(3);
            }
            if (items[j].name == "Confusion Scroll")
            {
                itemIDNumber.Add(4);
            }
            if (items[j].name == "Sword")
            {
                itemIDNumber.Add(101);
            }
            if (items[j].name == "Shield")
            {
                itemIDNumber.Add(102);
            }

            xposi.Add((int)items[j].transform.position.x);
            yposi.Add((int)items[j].transform.position.y);
        }

        SaveGame.Save<List<int>>("itemIDNumber", itemIDNumber);
        SaveGame.Save<List<int>>("xposi", xposi);
        SaveGame.Save<List<int>>("yposi", yposi);

        //stairs
        SaveGame.Save<int>("stairX", (int)stairsObj.transform.position.x);
        SaveGame.Save<int>("stairY", (int)stairsObj.transform.position.y);
        SaveGame.Save<bool>("stairSeen", stairsObj.GetComponent<Stairs>().hasBeenSeen);

        stopwatch.Stop();
        Debug.Log("Saving took:" + stopwatch.Elapsed);

        uiManager.NewMessage("Game Saved!");
    }

    public void LoadProgress()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();

        Debug.Log("Game Loaded");
        //player
        Vector3 playerLocation = SaveGame.Load<Vector3>("Player Location");
        CreatePlayer((int)playerLocation.x, (int)playerLocation.y, (int)playerLocation.z, 1, 1, 1);

        Fighter playerFighter = playerObject.GetComponent<Fighter>();

        playerFighter.MaxHP = SaveGame.Load<int>("maxHP");
        playerFighter.HP = SaveGame.Load<int>("hp");
        playerFighter.baseDefense = SaveGame.Load<int>("defense");
        playerFighter.basePower = SaveGame.Load<int>("power");

        Level playerLevel = playerObject.GetComponent<Level>();

        playerLevel.currentLevel = SaveGame.Load<int>("currentLevel");
        playerLevel.currentXP = SaveGame.Load<int>("currentXP");

        dungeonLevel = SaveGame.Load<int>("dungeonLevel");

        //uiManager.SetHealthText(SaveGame.Load<int>("hp"));
        uiManager.SetUIText();

        //inventory.items = SaveGame.Load<List<GameObject>>("Player Items");
        inventory.itemIDNumber = SaveGame.Load<List<int>>("Player Items");
        inventory.LoadItems();

        Equipment equipment = FindObjectOfType<Equipment>();
        equipment.eqIDNumber = SaveGame.Load<List<int>>("Player Eq");
        equipment.isEquippedList = SaveGame.Load<List<bool>>("isEquippedList");
        //equipment.LoadEquipped();


        //map
        tileSeenSingle = SaveGame.Load<bool[]>("tileSeen");
        tileVisibleSingle = SaveGame.Load<bool[]>("tileVisible");
        tileIsWallSingle = SaveGame.Load<bool[]>("tileIsWall");
        hasEntitySingle = SaveGame.Load<bool[]>("hasEntity");
        hasItemSingle = SaveGame.Load<bool[]>("hasItem");
        isWalkableSingle = SaveGame.Load<bool[]>("isWalkable");

        for (int i = 0; i < 3040; i++)
        {
            int x = i / gameMap.mapHeight;
            int y = i % gameMap.mapHeight;

            //Debug.Log(i + "," + x + "," + y);s

            gameMap.tileSeen[x, y] = tileSeenSingle[i];
            gameMap.tileVisible[x, y] = tileVisibleSingle[i];
            gameMap.tileIsWall[x, y] = tileIsWallSingle[i];
            //Debug.Log(tileIsWallSingle[i]);
            gameMap.hasEntity[x, y] = hasEntitySingle[i];
            gameMap.hasItem[x, y] = hasItemSingle[i];
            gameMap.isWalkable[x, y] = isWalkableSingle[i];
        }

        //gameMap.FillMap();
        gameMap.LoadTiles();
        //gameMap.MakeMap();



        //objects
        entities.Clear();
        List<int> xpos = SaveGame.Load<List<int>>("xpos");
        List<int> ypos = SaveGame.Load<List<int>>("ypos");
        List<int> entityNumber = SaveGame.Load<List<int>>("entityNumber");
        List<int> hpe = SaveGame.Load<List<int>>("hpe");

        for (int i = 0; i < xpos.Count; i++)
        {
            CreateEntity(xpos[i], ypos[i], entityNumber[i]);

            entities[i].GetComponent<Fighter>().HP = hpe[i];
        }


        //deadentities


        //items
        items.Clear();
        List<int> xposi = SaveGame.Load<List<int>>("xposi");
        List<int> yposi = SaveGame.Load<List<int>>("yposi");
        List<int> itemIDNumber = SaveGame.Load<List<int>>("itemIDNumber");

        for (int i = 0; i < xposi.Count; i++)
        {
            if(itemIDNumber[i] <100)
            {
                CreateItem(xposi[i], yposi[i], itemIDNumber[i]);
            }
            else if(itemIDNumber[i] < 200)
            {
                CreateEquipment(xposi[i], yposi[i], itemIDNumber[i]);
            }

        }

        equipment.LoadEquipped();

        //entities = SaveGame.Load<List<GameObject>>("entities");
        //deadEntities = SaveGame.Load<List<GameObject>>("deadEntities");
        //items = SaveGame.Load<List<GameObject>>("items");
        //equipment = SaveGame.Load<List<GameObject>>("equipment");


        //stairs

        int stairX = SaveGame.Load<int>("stairX");
        int stairY = SaveGame.Load<int>("stairY");
        bool stairSeen = SaveGame.Load<bool>("stairSeen");

        CreateStairs(new Vector2(stairX, stairY));
        stairsObj.GetComponent<Stairs>().hasBeenSeen = stairSeen;

        inventory.UpdatePlayerStats();
        gameMap.Render();
        stopwatch.Stop();
        Debug.Log("Loading took:" + stopwatch.Elapsed);
    }
}

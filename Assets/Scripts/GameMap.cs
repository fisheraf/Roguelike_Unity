using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Diagnostics;
using Debug = UnityEngine.Debug;
using TMPro;


public class GameMap : MonoBehaviour
{
    public Stopwatch timer;
    

    [SerializeField] Tilemap tilemap;
    [SerializeField] Engine engine;
    [SerializeField] Grid2 grid2;

    [Tooltip("Needs to be multiple of 2")]
    public int mapWidth = 90;//need to be multiples of 2
    [Tooltip("Needs to be multiple of 2")]
    public int mapHeight = 50;

    public int maxRoomSize = 10;
    public int minRoomSize = 5;
    public int maxNumberOfRooms = 50;

    // int maxNumberOfMonstersPerRoom = 4;
    //numer of monsters (key) after level (value)
    List<KeyValuePair<int, int>> monsterCountTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(2,1),
        new KeyValuePair<int, int>(3,4),
        new KeyValuePair<int, int>(5,6)
    };

    //public int maxNumberOfItemsPerRoom = 3;
    //likelihood of item (key) after level (value)
    List<KeyValuePair<int, int>> itemCountTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(1,1),
        new KeyValuePair<int, int>(2,4),
        new KeyValuePair<int, int>(3,6)
    };


    List<KeyValuePair<int, int>> goblinChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(900,0)
    };
    List<KeyValuePair<int, int>> orcChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(151,3),
        new KeyValuePair<int, int>(301,5),
        new KeyValuePair<int, int>(601,7)
    };


    List<KeyValuePair<int, int>> healingChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(500,0)
    }; 

    List<KeyValuePair<int, int>> lightningChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(0,0),
        new KeyValuePair<int, int>(250,4)
    };

    List<KeyValuePair<int, int>> fireballChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(1,0),
        new KeyValuePair<int, int>(249,6)
    };

    List<KeyValuePair<int, int>> confusionChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(2,0),
        new KeyValuePair<int, int>(100,2)
    };

    List<KeyValuePair<int, int>> swordChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(3,0),
        new KeyValuePair<int, int>(101,2)
    };

    List<KeyValuePair<int, int>> shieldChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(4,0),
        new KeyValuePair<int, int>(102,2)
    };


    //public int fovAlgorithm = 0;
    public bool fovLightWalls = true;
    public int fovRadius = 8;

    //bool fovRecompute = true;

    public bool debugShowAllTiles = false;

    [SerializeField] TileBase wallTiles = null;
    [SerializeField] TileBase floorTiles = null;

    public Color wallColor;
    public Color wallColorLit;
    public Color floorColor;
    public Color floorColorLit;

    //[SerializeField] Vector2[] wallLocations; //set by script...
    //[SerializeField] Vector2[] floorLocations;

    //public RectInt r;

    public GameObject player;

    
    //clean up needed...
    /*
    List<Vector3Int> seenTiles = new List<Vector3Int>();
    List<Vector3Int> recentTiles = new List<Vector3Int>();
    List<Vector3Int> previousTiles = new List<Vector3Int>();

    List<Vector3Int> visibleFloorTiles = new List<Vector3Int>();
    List<Vector3Int> visibleWallTiles = new List<Vector3Int>();
    List<Vector3Int> visibleTiles = new List<Vector3Int>();
    */

    //int numOfTiles;

    //List<bool> tileSeen = new List<bool>(4500); //remove hard code?
    //List<bool> tileVisible = new List<bool>(4500);
    //List<bool> tileIsWall = new List<bool>(4500);


    Cell[,] grid;
    public List<Cell> path;

    public bool[,] tileSeen = new bool[76, 40];
    public bool[,] tileVisible = new bool[76, 40];
    public bool[,] tileIsWall = new bool[76, 40];
    public bool[,] hasEntity = new bool [76, 40];
    public bool[,] hasItem = new bool[76, 40];
    public bool[,] isWalkable = new bool[76, 40];

    //bool is item

    public List<GameObject> entities = new List<GameObject>();
    public List<GameObject> deadEntities = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> equipment = new List<GameObject>();
    public GameObject stairs;

    public Dictionary<int, string> monsterDict = new Dictionary<int, string>
    {
        {900, "goblin" },
        {100, "orc" }
    };

    
    public Dictionary<int, string> itemDict = new Dictionary<int, string>
    {
        
        {500, "healing potion" },
        {0, "lightning scroll" },
        {1, "fireball scroll" },
        {2, "confusion scroll" },
        {3, "sword" },
        {4, "shield" }
        
    };


    private void Awake()
    {
        //FillMap();
        //MakeMap();
    }

    // Start is called before the first frame update
    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        engine = FindObjectOfType<Engine>();
        grid2 = FindObjectOfType<Grid2>();
        //set camera size from width & height?
        //PlaceTile();        

        //numOfTiles = mapHeight * mapWidth;
        //MakeMap();
        //r = new RectInt(botLeft, size);
        //CreateRoom(r);        
        //PlaceTile(8, 8, wallTiles);
        player = GameObject.Find("Player");
        //FOV2();

        //FillMap();
        //MakeMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        
        if(Input.GetKeyDown(KeyCode.M))
        {
            //NewMap();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            //ShowMap();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //engine.CreateEntity((int)player.transform.position.x + 2, (int)player.transform.position.y + 2, -1 , 1, new Color32(255, 0, 0, 255), "AI");
            //CreateGrid();
            
        }
        if(debugShowAllTiles)
        {
            ShowAllTiles();
            Render();
            //debugShowAllTiles = false;
        }
    }

    /*void PlaceTile()// X & Y?
    {
        for (int i = 0; i < wallLocations.Length; i++)
        {
            Instantiate(wallTiles, wallLocations[i], Quaternion.identity);
        }
        for (int i = 0; i < floorLocations.Length; i++)
        {
            Instantiate(floorTiles, floorLocations[i], Quaternion.identity);
        }
    }*/

    public void NewMap()
    {
        FillMap();
        HideAllTiles();
        ClearObjects();

        MakeMap();
        Render();
    }

    public void FillMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                PlaceTile(x, y, wallTiles);
                //Debug.Log(i * mapHeight + j);
                tileIsWall[x, y] = true; 
            }
        }
    }

    void ShowAllTiles()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tileSeen[x, y] = true;
                tileVisible[x, y] = true;
            }
        }
    }

    void HideAllTiles()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tileSeen[x, y] = false;
                tileVisible[x, y] = false;
                tilemap.SetColor(new Vector3Int(x, y, 0), Color.black);
            }
        }
    }

    void ClearObjects()
    {
        foreach (GameObject entity in entities)
        {
            Destroy(entity);
        }
        entities.Clear();
        foreach (GameObject deadEntity in deadEntities)
        {
            Destroy(deadEntity);
        }
        deadEntities.Clear();
        foreach (GameObject item in items)
        {
            Destroy(item);
        }
        items.Clear();
        foreach (GameObject equipment in equipment)
        {
            Destroy(equipment);
        }
        equipment.Clear();
    }

    void CreateGrid()
    {
        timer = new Stopwatch();
        timer.Start();

        grid = new Cell[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                bool walkable = !tileIsWall[x, y];
                Vector3 worldPoint = new Vector3(x, y, 0);
                //Debug.Log(worldPoint);
                grid[x, y] = new Cell(walkable, worldPoint, x, y);
                //Debug.Log(grid[x, y].gridX + ", " + grid[x, y].gridY);
            }
        }

        timer.Stop();
        Debug.Log("Creating grid took:" + timer.Elapsed);
    }

    /*void OnDrawGizmos()
    {
        //timer = new Stopwatch();
        //timer.Start();

        Gizmos.DrawWireCube(new Vector3(mapWidth/2, mapHeight/2, 0), new Vector3(mapWidth, mapHeight, 1));

        if (grid != null)
        {
            foreach (Cell n in grid)
            {
                Gizmos.color = (n.walkable) ? new Color(1,1,1,.3f) : new Color(1,0,0,.3f);
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = new Color(0,0,0,.3f);
                Gizmos.DrawCube(new Vector3(n.worldPosition.x +.5f, n.worldPosition.y + .5f,0), Vector3.one * (.9f));
            }
        }

        //timer.Stop();
        //Debug.Log("Creating gizmos took:" + timer.Elapsed);
    }*/

    int numberOfRooms = 0;

    public void MakeMap()
    {
        timer = new Stopwatch();
        timer.Start();

        List<Rect> rooms = new List<Rect>();
        numberOfRooms = 0;

        //level variety with random maxNumberOfRooms?

        for (int r = 0; r < maxNumberOfRooms; r++)
        {
            //random room size
            int w = Random.Range(minRoomSize, maxRoomSize);
            int h = Random.Range(minRoomSize, maxRoomSize);
            //random position inside map
            int x = Random.Range(1, mapWidth - w - 1);//stopped left side edge cases
            int y = Random.Range(0, mapHeight - h - 2);//stopped top side edge cases

            //added spacing between rooms
            Rect newRoom = new Rect(x - 1, y + 1, w + 2, h + 2);

            bool overlapping = false;
            for (int i = 0; i < rooms.Count; i++)
            {
                if (newRoom.Overlaps(rooms[i]))
                {
                    overlapping = true;
                    break;
                }
            }
            if(!overlapping)
            {
                RectInt newRoomRect = new RectInt(Mathf.RoundToInt(newRoom.xMin + 1), 
                    Mathf.RoundToInt(newRoom.yMin + 1),
                    Mathf.RoundToInt(newRoom.width - 2), 
                    Mathf.RoundToInt(newRoom.height - 2));
                //StartCoroutine(CreateRoomD(newRoomInt));
                CreateRoom(newRoomRect);

                //center of room for tunnels
                int newX = Mathf.RoundToInt(newRoomRect.center.x);
                int newY = Mathf.RoundToInt(newRoomRect.center.y);


                if (numberOfRooms == 0)
                {
                    //engine.CreateEntity(newX, newY, -1, 0, new Color32(0, 135, 255, 255), "Player");

                    if(player == null)
                    {
                        engine.CreatePlayer(newX, newY, -1, 100, 1, 4);
                    }
                    else
                    {
                        player.transform.position = new Vector3(newX, newY, -1);
                    }
                    player = GameObject.Find("Player");
                    
                    //if(player == null)
                    //{
                    //    player = GameObject.Find("Player");
                    //}
                    //player.transform.position = new Vector3(newX, newY, -1);
                }
                else
                {
                    //connect rooms with tunnel
                    //change to update old room loc until after tunnels made?
                    int prevX = Mathf.RoundToInt(rooms[(numberOfRooms - 1)].center.x);
                    int prevY = Mathf.RoundToInt(rooms[(numberOfRooms - 1)].center.y);

                    //randomly pick tunnel direction
                    if (Random.Range(0, 1) == 1)
                    {

                        //StartCoroutine(CreateVerticalTunnel(prevX, newX, prevY));
                        //StartCoroutine(CreateHorizontalTunnel(prevY, newY, newX));

                        CreateVerticalTunnel(prevX, newX, prevY);
                        CreateHorizontalTunnel(prevY, newY, newX);
                    }
                    else
                    {
                        //StartCoroutine(CreateVerticalTunnel(prevY, newY, prevX));
                        //StartCoroutine(CreateHorizontalTunnelD(prevX, newX, newY));

                        CreateVerticalTunnel(prevY, newY, prevX);
                        CreateHorizontalTunnel(prevX, newX, newY);
                    }
                }

                //PlaceEntities(newRoomInt);

                PlaceEntities(newRoomRect);
                PlaceItems(newRoomRect);


                rooms.Add(newRoom);
                numberOfRooms++;
            }
            //yield return new WaitForSeconds(.1f);
        }

        if(rooms.Count > 1)
        {
            Vector2 lastRoom = rooms[rooms.Count - 1].center;
            engine.CreateStairs(lastRoom);
        }

        timer.Stop();
        Debug.Log("Creating map took:" + timer.Elapsed);
        //FOVRecompute();
        FOV();
        //GetVisibleCells();
    }
    

    void CreateRoom(RectInt rectInt)
    {
        for (int x = rectInt.xMin; x < rectInt.xMax; x++)
        {
            for (int y = rectInt.yMin; y < rectInt.yMax; y++)
            {
                PlaceTile(x, y, floorTiles);
                tileIsWall[x, y] = false;
                //grid[x, y] = new Cell(true, new Vector3(x, y, 0), x, y);
                //StartCoroutine(PlaceTileWithDelay(i, j, floorTiles));
            }
        }
        //PlaceEntities(rectInt);
        //PlaceItems(rectInt);
    }
    void CreateHorizontalTunnel(int x1, int x2, int y)
    {
        for (int x = Mathf.Min(x1, x2); x < Mathf.Max(x1, x2) + 1; x++)
        {
            PlaceTile(x, y, floorTiles);
            tileIsWall[x, y] = false;
            //grid[x, y] = new Cell(true, new Vector3(x, y, 0), x, y);
            //StartCoroutine(PlaceTileWithDelay(x, y, floorTiles));
        }
    }

    void CreateVerticalTunnel(int y1, int y2, int x)
    {
        for (int y = Mathf.Min(y1, y2); y < Mathf.Max(y1, y2) + 1; y++)
        {
            PlaceTile(x, y, floorTiles);
            tileIsWall[x, y] = false;
            //grid[x, y] = new Cell(true, new Vector3(x, y, 0), x, y);

            //StartCoroutine(PlaceTileWithDelay(x, y, floorTiles));
        }
    }

    public void PlaceTile(int x, int y, TileBase tile)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        tilemap.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
    }

    IEnumerator CreateRoomD(RectInt rectInt)
    {
        for (int i = rectInt.xMin; i < rectInt.xMax; i++)
        {
            for (int j = rectInt.yMin; j < rectInt.yMax; j++)
            {
                PlaceTile(i, j, floorTiles);
                yield return new WaitForSeconds(.1f);
                //StartCoroutine(PlaceTileWithDelay(i, j, floorTiles));
            }
        }
    }

    IEnumerator CreateHorizontalTunnelD(int x1, int x2, int y)
    {
        for (int x   = Mathf.Min(x1, x2); x < Mathf.Max(x1, x2) + 1; x++)
        {
            PlaceTile(x, y, floorTiles);
            yield return new WaitForSeconds(.1f);
            //StartCoroutine(PlaceTileWithDelay(x, y, floorTiles));
        }
    }
       
    IEnumerator PlaceTileWithDelay(int x, int y, TileBase tile)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        yield return new WaitForSeconds(1);
    }
         

    float degToRad = Mathf.PI / 100;

    public int DiagDistance(int x, int y, int x1, int y1)
    {
        /*
        int dx = x1 - x;
        int dy = y1 - y;
        return  Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
        */

        return (int)Vector2.Distance(new Vector2(x, y), new Vector2(x1, y1));
    }

    public void FOV()
    {
        //Debug.Log("FOV");
        ClearVisibleTiles();
        //timer = new Stopwatch();
        //timer.Start();

        //visibleWallTiles.Clear();
        //visibleFloorTiles.Clear();

        //previousTiles.AddRange(recentTiles);

        //recentTiles.Clear();
        //tileVisible = new bool[5185];


        int x = Mathf.RoundToInt(player.transform.position.x);
        int y = Mathf.RoundToInt(player.transform.position.y);

        for (int i = 0; i < 360; i++)//drop to every 3rd?
        {
            float deg = i * degToRad;

            int endX = Mathf.RoundToInt(Mathf.Cos(deg) * fovRadius + x);
            int endY = Mathf.RoundToInt(Mathf.Sin(deg) * fovRadius + y);

            int d = DiagDistance(x, y, endX, endY);

            for (int j = 0; j < d; j++)
            {
                int tx = Mathf.RoundToInt(Mathf.Lerp(x, endX, j / Mathf.Round(d)));
                int ty = Mathf.RoundToInt(Mathf.Lerp(y, endY, j / Mathf.Round(d)));

                if (tx < 0 || tx > mapWidth) { continue; }
                if (ty < 0 || ty > mapWidth) { continue; }

                if (tilemap.GetTile(new Vector3Int(tx, ty, 0)).name == "WallTile")
                {
                    //visibleWallTiles.Add(new Vector3Int(tx, ty, 0));
                    //seenTiles.Add(new Vector3Int(tx, ty, 0));
                    tileVisible[tx, ty] = true;
                    tileSeen[tx, ty] = true;
                    break;
                }

                //visibleFloorTiles.Add(new Vector3Int(x, y, 0));
                //seenTiles.Add(new Vector3Int(tx, ty, 0));

                tileVisible[tx, ty] = true;
                tileSeen[tx, ty] = true;

                //recentTiles.Add(new Vector3Int(tx, ty, 0));
            }
        }

        //RenderRecent();

        //timer.Stop();
        //Debug.Log("Rendering took:" + timer.Elapsed);
        //previousTiles.Clear();

        RayCastPostProcess();

        Render();
    }

    
    void ClearVisibleTiles()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tileVisible[x, y] = false;
                hasEntity[x, y] = false;
                isWalkable[x, y] = true;

                /*foreach (GameObject entity in entities)
                {
                    if (entity.transform.position.z == -1 && entity.name != "Player")
                    {
                        Debug.Log("move forward" + entity.transform.position + "," + entity.name);
                        //entity.transform.position = new Vector3(entity.transform.position.x, entity.transform.position.y, 1);
                        //entity.GetComponentInChildren<GameObject>().SetActive(false);
                    }
                }*/
            }
        }
    }

    void RayCastPostProcess()
    {
        int x = (int)player.transform.position.x;
        int y = (int)player.transform.position.y;

        int z = x * mapHeight + y;
        //Debug.Log("X:" + x + " Y:" + y + " Z:" + z + " W:" + mapWidth + " H:" + mapHeight);

        if (y < mapHeight - 5)
        {
            if (tileVisible[x, y + 1] == true && tileVisible[x, y + 2] == true && tileVisible[x, y + 3] == true)
            {
                //Debug.Log("added tile: " + (z + 4 + mapHeight);
                tileVisible[x + 1, y + 4] = true;
                tileVisible[x + 1, y + 5] = true;
                tileVisible[x - 1, y + 4] = true;
                tileVisible[x - 1, y + 5] = true;
            }
        }

        if (y > 5)
        {
            if (tileVisible[x, y - 1] == true && tileVisible[x, y - 2] == true && tileVisible[x, y - 3] == true)
            {
                //Debug.Log("added tile: " + (z + 4 + mapHeight);
                tileVisible[x + 1, y - 4] = true;
                tileVisible[x + 1, y - 5] = true;
                tileVisible[x - 1, y - 4] = true;
                tileVisible[x - 1, y - 5] = true;
            }
        }

        if (x < mapWidth - 5)
        {
            if (tileVisible[x + 1, y] == true && tileVisible[x + 2, y] == true && tileVisible[x + 3, y] == true)
            {
                //Debug.Log("added tile: " + (z + 4 + mapHeight);
                tileVisible[x + 4, y + 1] = true;
                tileVisible[x + 5, y + 1] = true;
                tileVisible[x + 4, y - 1] = true;
                tileVisible[x + 5, y - 1] = true;
            }
        }

        if(x > 5)
        {
            if (tileVisible[x + 1, y] == true && tileVisible[x - 2, y] == true && tileVisible[x - 3, y] == true)
            {
                //Debug.Log("added tile: " + (z + 4 + mapHeight);
                tileVisible[x - 4, y + 1] = true;
                tileVisible[x - 5, y + 1] = true;
                tileVisible[x - 4, y - 1] = true;
                tileVisible[x - 5, y - 1] = true;
            }
        }


        //tileVisible[tx * mapHeight + ty] = true;
        //tileSeen[tx * mapHeight + ty] = true;
    }
         

    public void Render()
    {
        ResetEntitiesRender();
        //Debug.Log("render");
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //Debug.Log(x + ", " + y);
                if (tileSeen[x, y])
                {
                    //Debug.Log(i + "seen");
                    if (tileVisible[x, y])
                    {
                        if (tileIsWall[x, y])
                        {
                            tilemap.SetColor(new Vector3Int(x, y, 0), wallColorLit);
                        }
                        else
                        {
                            tilemap.SetColor(new Vector3Int(x, y, 0), floorColorLit);
                        }

                        foreach (GameObject entity in entities)
                        {
                            hasEntity[(int)entity.transform.position.x, (int)entity.transform.position.y] = true;
                            if (entity.transform.position.x == x && entity.transform.position.y == y)
                            {
                                //entity.transform.position = new Vector3(x, y, -1);
                                //entity.GetComponentInChildren<SpriteRenderer>().enabled = true;
                                entity.GetComponentInChildren<TextMeshPro>().enabled = true;
                            }
                        }
                        foreach(GameObject deadEntity in deadEntities)
                        {
                            //hasEntity[(int)deadEntity.transform.position.x, (int)deadEntity.transform.position.y] = true;
                            if (deadEntity.transform.position.x == x && deadEntity.transform.position.y == y)
                            {
                                deadEntity.GetComponentInChildren<TextMeshPro>().enabled = true;
                            }
                        }

                        foreach(GameObject item in items)
                        {
                            hasItem[(int)item.transform.position.x, (int)item.transform.position.y] = true;
                            if (item.transform.position.x == x && item.transform.position.y == y)
                            {
                                item.GetComponentInChildren<SpriteRenderer>().enabled = true;
                            }
                        }

                        if (stairs.transform.position.x == x && stairs.transform.position.y == y)
                        {
                            stairs.GetComponent<Stairs>().hasBeenSeen = true;
                        }

                    }
                    else
                    {
                        if (tileIsWall[x, y])
                        {
                            tilemap.SetColor(new Vector3Int(x, y, 0), wallColor);
                        }
                        else
                        {
                            tilemap.SetColor(new Vector3Int(x, y, 0), floorColor);
                        }
                    }

                }
            }
        }
        if (stairs.GetComponent<Stairs>().hasBeenSeen == true)
        {
            stairs.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
        grid2.CreateGrid();
    }

    public void UpdateWalkable()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                isWalkable[x, y] = true;

                foreach (GameObject entity in entities)
                {
                    hasEntity[(int)entity.transform.position.x, (int)entity.transform.position.y] = true;
                }

                if (tileIsWall[x, y] || hasEntity[x, y])
                {
                    isWalkable[x, y] = false;
                }
            }
        }
    }

    public void LoadTiles()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //Debug.Log(x + "," + y);
                if(tileIsWall[x,y] == true)
                {
                    PlaceTile(x, y, wallTiles);
                }
                else
                {
                    //Debug.Log("floor tile placed");
                    PlaceTile(x, y, floorTiles);
                }

                if(!tileSeen[x,y])
                {
                    tilemap.SetColor(new Vector3Int(x, y, 0), Color.black);
                }
            }
        }
    }

    void ResetEntitiesRender()
    {
        foreach (GameObject entity in entities)
        {            
            {
                //moved entities to z = 1 to remove from camera
                //entity.transform.position = new Vector3(entity.transform.position.x, entity.transform.position.y, 1);
                //entity.GetComponentInChildren<SpriteRenderer>().enabled = false;
                //entity.GetComponentInChildren<TextMeshPro>().enabled = false;
                entity.GetComponentInChildren<TextMeshPro>().enabled = false;
                //entity.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }
        }
        foreach (GameObject deadEntity in deadEntities)
        {
            deadEntity.GetComponentInChildren<TextMeshPro>().enabled = false;
        }
        foreach (GameObject item in items)
        {
            item.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }
    
 

    private int RandomChoiceIndex(List<int> chances)
    {
        int randomChance = Random.Range(1, chances.Sum());

        int runningSum = 0;
        int choice = 0;
        for (int i = 0; i < chances.Count; i++)
        {
            runningSum += chances[i];

            if (randomChance <= runningSum)
            {
                return choice;
            }
            choice += 1;
        }
        return 0;
    }

    //key is %chance and value is chance selection(monst/item)
    private string RandomChoiceFromDict(Dictionary<int, string> choiceDict)
    {
        List<int> chances = choiceDict.Keys.ToList();
        List<string> choices = choiceDict.Values.ToList();

        return choices[RandomChoiceIndex(chances)];
    }

    //key is %chance and value is dungeon level
    public int FromDungeonLevel(List<KeyValuePair<int, int>> table)
    {
        for (int i = table.Count - 1; i >= 0; i--)
        {
            if (table[i].Value <= engine.dungeonLevel)
            {
                return table[i].Key;
            }
        }
        return 0;
    }

    public void UpdateDicts()
    {
        monsterDict.Clear();  
        monsterDict.Add(FromDungeonLevel(goblinChanceTable), "goblin");
        monsterDict.Add(FromDungeonLevel(orcChanceTable), "orc");

        itemDict.Clear();
        itemDict.Add(FromDungeonLevel(healingChanceTable), "healing potion");
        itemDict.Add(FromDungeonLevel(lightningChanceTable), "lightning scroll");
        itemDict.Add(FromDungeonLevel(fireballChanceTable), "fireball scroll");
        itemDict.Add(FromDungeonLevel(confusionChanceTable), "confusion scroll");
        itemDict.Add(FromDungeonLevel(swordChanceTable), "sword");
        itemDict.Add(FromDungeonLevel(shieldChanceTable), "shield");
    }


    public void PlaceEntities(RectInt rectInt)
    {
        int maxNumberOfMonstersPerRoom = FromDungeonLevel(monsterCountTable);
        int numberOfMonsters = Random.Range(0, maxNumberOfMonstersPerRoom + 1);

        for (int i = 0; i <= numberOfMonsters; i++)
        {
            int x = Random.Range(rectInt.x, rectInt.x + rectInt.width);
            int y = Random.Range(rectInt.y, rectInt.y + rectInt.height);

            if (hasEntity[x, y] || (player.transform.position.x == x && player.transform.position.y == y)) return;

            {
                /*
                if(Random.Range(0,100) < 20)
                {
                    //engine.CreateEntity(x, y, 1, 2, new Color32(0, 80, 0, 255), "Orc", 10, 1, 3);
                    engine.CreateEntity(x, y, 1);
                }
                else
                {
                    //.CreateEntity(x, y, 1, 2, new Color32(0, 160, 0, 255), "Goblin", 5, 0, 2);
                    engine.CreateEntity(x, y, 0);
                }
                */
                string monsterChoice = RandomChoiceFromDict(monsterDict);
                if(monsterChoice == "orc")
                {
                    engine.CreateEntity(x, y, 1);
                }
                else
                {
                    engine.CreateEntity(x, y, 0);
                }
            }

        }
    }

    public void PlaceItems(RectInt rectInt)
    {
        //int numberOfItems = 10;//Random.Range(0, maxNumberOfItemsPerRoom);
        int maxNumberOfItemsPerRoom = FromDungeonLevel(itemCountTable);
        int numberOfItems = Random.Range(0, maxNumberOfItemsPerRoom + 1);

        for (int i = 0; i <= numberOfItems; i++)
        {
            int x = Random.Range(rectInt.x, rectInt.x + rectInt.width);
            int y = Random.Range(rectInt.y, rectInt.y + rectInt.height);

            if (hasItem[x, y]) return;

            /*
            {
                int itemChance = Random.Range(0, 500);

                if (itemChance < 25)
                {
                    engine.CreateItem(x, y, 1);

                }
                else if (itemChance < 50)
                {
                    engine.CreateItem(x, y, 2);

                }
                else if (itemChance < 75)
                {
                    engine.CreateItem(x, y, 3);
                }
                else
                {
                    engine.CreateItem(x, y, 4);
                }
            }
            */

            string itemChoice = RandomChoiceFromDict(itemDict);
            //Debug.Log(itemChoice);

            if (itemChoice == "healing potion")
            {
                engine.CreateItem(x, y, 1);
            }
            else if (itemChoice == "lightning scroll")
            {
                engine.CreateItem(x, y, 2);
            }
            else if (itemChoice == "fireball scroll")
            {
                engine.CreateItem(x, y, 3);
            }
            else if(itemChoice == "confusion scroll")
            {
                engine.CreateItem(x, y, 4);
            }

            else if(itemChoice == "sword")
            {
                engine.CreateEquipment(x, y, 101);
            }
            else if(itemChoice == "shield")
            {
                engine.CreateEquipment(x, y, 102);
            }
        }
    }   

    public void ShowMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "FloorTile")
                {
                    tilemap.SetColor(new Vector3Int(x, y, 0), floorColor);

                }
                else
                {
                    tilemap.SetColor(new Vector3Int(x, y, 0), wallColor);
                }
            }
        }
    }


    public List<Cell> neighbors = new List<Cell>();
    public List<Vector2> neighbours = new List<Vector2>();

    public List<Cell> GetNeighbors(Cell cell)
    {
        neighbors = new List<Cell>();
        neighbours = new List<Vector2>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = cell.gridX + x;
                int checkY = cell.gridY + y;

                if (checkX >= 0 && checkX < mapWidth && checkY >= 0 && checkY < mapHeight)
                {
                    neighbors.Add(grid[checkX, checkY]);
                    neighbours.Add(new Vector2(checkX, checkY));
                }
            }
        }
        return neighbors;
    }

    public Cell CellFromWorldPoint(Vector3 worldPosition)
    {
        int x = (int)worldPosition.x;
        int y = (int)worldPosition.y;
        return grid[x, y];
    }

    #region Other FOV Attempts


    /*
    public void FOVRecompute()
    {        
        visibleWallTiles.Clear();
        visibleFloorTiles.Clear();

        previousTiles.AddRange(recentTiles);

        recentTiles.Clear();

        //Debug.Log("FOV Recompute");
        //tilemap.SetColor(new Vector3Int(1, 1, 0), Color.blue);
        tilemap.RefreshTile(new Vector3Int(1, 1, 1));
        for (int x = Mathf.RoundToInt(player.transform.position.x - 3); x < Mathf.RoundToInt(player.transform.position.x + 3); x++)
        {
            for (int y = Mathf.RoundToInt(player.transform.position.y + 3); y > Mathf.RoundToInt(player.transform.position.y - 3); y--)
            {
                if(tilemap.GetTile(new Vector3Int(x,y,0)).name == "FloorTile")
                {
                    //tilemap.SetColor(new Vector3Int(x, y, 0), floorColorLit);
                    //Debug.Log(x + "," + y);
                    //add to seen list
                    visibleWallTiles.Add(new Vector3Int(x, y, 0));                    
                }
                else
                {
                    //tilemap.SetColor(new Vector3Int(x, y, 0), wallColorLit);
                    
                    //add to seen list
                    visibleFloorTiles.Add(new Vector3Int(x, y, 0)); 
                }

                if(!seenTiles.Contains(new Vector3Int(x, y, 0)))
                {
                    seenTiles.Add(new Vector3Int(x, y, 0));
                }
                recentTiles.Add(new Vector3Int(x, y, 0));
            }
        }
        //RenderAll();
        RenderRecent();
    }

    public void RenderAll()
    {
        //only set a limited number of tiles for performance (1 outside of fov?)
        //Debug.Log("render");
        timer = new Stopwatch();
        timer.Reset();
        timer.Start();

        for (int x = 0; x < mapWidth; x++)
        {
            //Debug.Log(x);
            for (int y = 0; y < mapHeight; y++)
            {
                //Debug.Log(y);
                if (seenTiles.Contains(new Vector3Int (x, y, 0)))
                {
                    if(visibleFloorTiles.Contains(new Vector3Int(x, y, 0)))
                    {
                        tilemap.SetColor(new Vector3Int(x, y, 0), floorColorLit);
                    }
                    else if(visibleWallTiles.Contains(new Vector3Int(x, y, 0)))
                    {
                        tilemap.SetColor(new Vector3Int(x, y, 0), wallColorLit);
                    }
                    else
                    {
                        if (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "FloorTile")
                        {
                            tilemap.SetColor(new Vector3Int(x, y, 0), floorColor);
                        }
                        else
                        {
                            tilemap.SetColor(new Vector3Int(x, y, 0), wallColor);
                        }
                    }
                }                
            }
        }
        timer.Stop();
        Debug.Log(timer.Elapsed);
    }

    public void RenderRecent()
    {
        timer = new Stopwatch();
        timer.Reset();
        timer.Start();

        foreach (Vector3Int vector3Int in previousTiles)
        {
            //Debug.Log(y);
            //if (seenTiles.Contains(vector3Int))
            {
                if (visibleFloorTiles.Contains(vector3Int))
                {
                    tilemap.SetColor(vector3Int, floorColorLit);
                }
                else if (visibleWallTiles.Contains(vector3Int))
                {
                    tilemap.SetColor(vector3Int, wallColorLit);
                }
                else
                {
                    if (tilemap.GetTile(vector3Int).name == "FloorTile")
                    {
                        tilemap.SetColor(vector3Int, floorColor);
                    }
                    else
                    {
                        tilemap.SetColor(vector3Int, wallColor);
                    }
                }
            }
        }
        
        timer.Stop();
        Debug.Log(timer.Elapsed);
        previousTiles.Clear();
    }

    
    public void Compute(Vector3Int origin, int rangeLimit)
    {

        visibleTiles.Add(new Vector3Int());
    }

    public int[,] mult = new int[,]
    {       
        { 1, 0, 0, -1, -1, 0, 0, 1 },
        { 0, 1, -1, 0, 0, -1, 1, 0 },
        { 0, 1, 1, 0, 0, -1, -1, 0 },
        { 1, 0, 0, 1, -1, 0, 0, -1 }
    };

    public void FOV2()
    {
        Debug.Log(mult[0,7]);

        //CastLight();
        //ComputeRecursiveShadowCast();
    }


    public List<Vector2> VisiblePoints;
    List<int> VisibleOctants = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

    public void GetVisibleCells()
    {
        VisiblePoints = new List<Vector2>();
        foreach (int o in VisibleOctants)
            ScanOctant(1, o, 1.0, 0.0);
    }

    void ScanOctant(int pDepth, int pOctant, double pStartSlope, double pEndSlope)
    {
        int visRange2 = fovRadius * fovRadius;
        int x = 0;
        int y = 0;

        switch (pOctant)
        {
            case 1: //nnw
                y = Mathf.RoundToInt(player.transform.position.y) - pDepth;
                if (y < 0) return;

                x = Mathf.RoundToInt(player.transform.position.x) - (int)pStartSlope * pDepth;
                if (x < 0) x = 0;

                Debug.Log(x + "&" + y);

                /*
                Debug.Log(pEndSlope);
                Debug.Log(GetSlope(x, y, (double)player.transform.position.x, (double)player.transform.position.y, false));

                Debug.Log(GetVisDistance(x, y, (int)player.transform.position.x, (int)player.transform.position.y));
                Debug.Log(visRange2);
                */
    /*

while (GetSlope(x, y, (double)player.transform.position.x, (double)player.transform.position.y, false) >= pEndSlope)
{
if(GetVisDistance(x,y, (int)player.transform.position.x, (int)player.transform.position.y) <= visRange2)
{
if(tilemap.GetTile(new Vector3Int(x, y, 0)).name == "WallTile")
{
    Debug.Log("wall");
    if (x - 1 >= 0 && tilemap.GetTile(new Vector3Int(x - 1, y, 0)).name != "WallTile")
    {
        Debug.Log("floor");
        ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, (double)player.transform.position.x, (double)player.transform.position.y, false));
    }
}
else
{
    if(x - 1 >= 0 && tilemap.GetTile(new Vector3Int(x - 1, y, 0)).name == "WallTile")
    {
        pStartSlope = GetSlope(x - 0.5, y - 0.5, (double)player.transform.position.x, (double)player.transform.position.y, false);

        VisiblePoints.Add(new Vector2(x, y));
    }
}
}
x++;
}
x--;
break;


case 2: //nne
y = Mathf.RoundToInt(player.transform.position.y) - pDepth;
if (y < 0) return;

x = Mathf.RoundToInt(player.transform.position.x) + (int)pStartSlope * pDepth;
if (x >= mapWidth) x = mapWidth - 1;

while (GetSlope(x, y, (double)player.transform.position.x, (double)player.transform.position.y, false) <= pEndSlope)
{
if (GetVisDistance(x, y, (int)player.transform.position.x, (int)player.transform.position.y) <= visRange2)
{
if (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "WallTile")
{
    if (x + 1 < mapWidth && tilemap.GetTile(new Vector3Int(x + 1, y, 0)).name != "WallTile")
    {
        ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, (double)player.transform.position.x, (double)player.transform.position.y, false));
    }
}
else
{
    if (x + 1 < mapWidth && tilemap.GetTile(new Vector3Int(x + 1, y, 0)).name == "WallTile")
    {
        pStartSlope = -GetSlope(x + 0.5, y - 0.5, (double)player.transform.position.x, (double)player.transform.position.y, false);

        VisiblePoints.Add(new Vector2(x, y));
    }
}
}
x--;
}
x++;
break;


case 3: 
x = Mathf.RoundToInt(player.transform.position.x) + pDepth;
if (x >= mapWidth) return;

y = Mathf.RoundToInt(player.transform.position.y) - (int)pStartSlope * pDepth;
if (y < 0) y = 0;

while (GetSlope(x, y, (double)player.transform.position.x, (double)player.transform.position.y, true) <= pEndSlope)
{
if (GetVisDistance(x, y, (int)player.transform.position.x, (int)player.transform.position.y) <= visRange2)
{
if (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "WallTile")
{
    if (y - 1 >= 0 && tilemap.GetTile(new Vector3Int(x, y - 1, 0)).name != "WallTile")
    {
        ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, (double)player.transform.position.x, (double)player.transform.position.y, true));
    }
}
else
{
    if (y - 1 >= 0 && tilemap.GetTile(new Vector3Int(x, y - 1, 0)).name == "WallTile")
    {
        pStartSlope = -GetSlope(x + 0.5, y - 0.5, (double)player.transform.position.x, (double)player.transform.position.y, true);

        VisiblePoints.Add(new Vector2(x, y));
    }
}
}
y++;
}
y--;
break;


case 4:
x = Mathf.RoundToInt(player.transform.position.x) + pDepth;
if (x >= mapWidth) return;

y = Mathf.RoundToInt(player.transform.position.y) + (int)pStartSlope * pDepth;
if (y >= mapHeight) y = mapHeight - 1;

while (GetSlope(x, y, (double)player.transform.position.x, (double)player.transform.position.y, true) >= pEndSlope)
{
if (GetVisDistance(x, y, (int)player.transform.position.x, (int)player.transform.position.y) <= visRange2)
{
if (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "WallTile")
{
    if (y + 1 < mapHeight && tilemap.GetTile(new Vector3Int(x, y + 1, 0)).name != "WallTile")
    {
        ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, (double)player.transform.position.x, (double)player.transform.position.y, true));
    }
}
else
{
    if (y + 1 < mapHeight && tilemap.GetTile(new Vector3Int(x, y + 1, 0)).name == "WallTile")
    {
        pStartSlope = GetSlope(x + 0.5, y + 0.5, (double)player.transform.position.x, (double)player.transform.position.y, true);

        VisiblePoints.Add(new Vector2(x, y));
    }
}
}
y--;
}
y++;
break;


case 5:
y = Mathf.RoundToInt(player.transform.position.y) + pDepth;
if (y >= mapHeight) return;

x = Mathf.RoundToInt(player.transform.position.x) + (int)pStartSlope * pDepth;
if (x >= mapWidth) x = mapWidth - 1;

while (GetSlope(x, y, (double)player.transform.position.x, (double)player.transform.position.y, false) >= pEndSlope)
{
if (GetVisDistance(x, y, (int)player.transform.position.x, (int)player.transform.position.y) <= visRange2)
{
if (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "WallTile")
{
    if (x + 1 < mapWidth && tilemap.GetTile(new Vector3Int(x + 1, y, 0)).name != "WallTile")
    {
        ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, (double)player.transform.position.x, (double)player.transform.position.y, false));
    }
}
else
{
    if (x + 1 < mapWidth && tilemap.GetTile(new Vector3Int(x + 1, y, 0)).name == "WallTile")
    {
        pStartSlope = GetSlope(x + 0.5, y + 0.5, (double)player.transform.position.x, (double)player.transform.position.y, false);

        VisiblePoints.Add(new Vector2(x, y));
    }
}
}
x--;
}
x++;
break;


case 6:
y = Mathf.RoundToInt(player.transform.position.y) + pDepth;
if (y >= mapHeight) return;

x = Mathf.RoundToInt(player.transform.position.x) - (int)pStartSlope * pDepth;
if (x < 0 ) x = 0;

while (GetSlope(x, y, (double)player.transform.position.x, (double)player.transform.position.y, false) <= pEndSlope)
{
if (GetVisDistance(x, y, (int)player.transform.position.x, (int)player.transform.position.y) <= visRange2)
{
if (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "WallTile")
{
    if (x - 1 >= 0 && tilemap.GetTile(new Vector3Int(x - 1, y, 0)).name != "WallTile")
    {
        ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, (double)player.transform.position.x, (double)player.transform.position.y, false));
    }
}
else
{
    if (x - 1 >= 0 && tilemap.GetTile(new Vector3Int(x - 1, y, 0)).name == "WallTile")
    {
        pStartSlope = -GetSlope(x - 0.5, y + 0.5, (double)player.transform.position.x, (double)player.transform.position.y, false);

        VisiblePoints.Add(new Vector2(x, y));
    }
}
}
x++;
}
x--;
break;


case 7:
x = Mathf.RoundToInt(player.transform.position.x) - pDepth;
if (x < 0) return;

y = Mathf.RoundToInt(player.transform.position.y) + (int)pStartSlope * pDepth;
if (y >= mapHeight) y = mapHeight - 1;

while (GetSlope(x, y, (double)player.transform.position.x, (double)player.transform.position.y, true) <= pEndSlope)
{
if (GetVisDistance(x, y, (int)player.transform.position.x, (int)player.transform.position.y) <= visRange2)
{
if (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "WallTile")
{
    if (y + 1 < mapHeight && tilemap.GetTile(new Vector3Int(x, y + 1, 0)).name != "WallTile")
    {
        ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, (double)player.transform.position.x, (double)player.transform.position.y, true));
    }
}
else
{
    if (y + 1 < mapHeight && tilemap.GetTile(new Vector3Int(x, y + 1, 0)).name == "WallTile")
    {
        pStartSlope = GetSlope(x - 0.5, y + 0.5, (double)player.transform.position.x, (double)player.transform.position.y, true);

        VisiblePoints.Add(new Vector2(x, y));
    }
}
}
y--;
}
y++;
break;


case 8:
x = Mathf.RoundToInt(player.transform.position.x) - pDepth;
if (x < 0) return;

y = Mathf.RoundToInt(player.transform.position.y) - (int)pStartSlope * pDepth;
if (y < 0) y = 0;

while (GetSlope(x, y, (double)player.transform.position.x, (double)player.transform.position.y, true) >= pEndSlope)
{
if (GetVisDistance(x, y, (int)player.transform.position.x, (int)player.transform.position.y) <= visRange2)
{
if (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "WallTile")
{
    if (y - 1 >= 0 && tilemap.GetTile(new Vector3Int(x, y - 1, 0)).name != "WallTile")
    {
        ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, (double)player.transform.position.x, (double)player.transform.position.y, true));
    }
}
else
{
    if (x + 1 < mapHeight && tilemap.GetTile(new Vector3Int(x, y - 1, 0)).name == "WallTile")
    {
        pStartSlope = GetSlope(x - 0.5, y - 0.5, (double)player.transform.position.x, (double)player.transform.position.y, true);

        VisiblePoints.Add(new Vector2(x, y));
    }
}
}
y++;
}
y--;
break;
}

if (x < 0)
{
x = 0;
}
else if (x >= mapWidth)
{
x = mapWidth - 1;
}

if (y < 0)
{
y = 0;
}
else if(y >= mapHeight)
{
y = mapHeight - 1;
}

if (pDepth < fovRadius & tilemap.GetTile(new Vector3Int(x, y, 0)).name != "WallTile")
{
ScanOctant(pDepth + 1, pOctant, pStartSlope, pEndSlope);
}
Render2();
}

double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert)
{
if (pInvert)
return (pY1 - pY2) / (pX1 - pX2);
else
return (pX1 - pX2) / (pY1 - pY2);    
}

int GetVisDistance(int pX1, int pY1, int pX2, int pY2)
{
return ((pX1 - pX2) * (pX1 - pX2)) + ((pY1 - pY2) * (pY1 - pY2));
}

public void Render2()
{
foreach (Vector2 item in VisiblePoints)
{
if (tilemap.GetTile(new Vector3Int((int)item.x, (int)item.y, 0)).name == "WallTile")
{
tilemap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), wallColorLit);
}
else
{
tilemap.SetColor(new Vector3Int((int)item.x, (int)item.y, 0), floorColorLit);
}
}
}

public void AddLit(int x, int y)
{
VisiblePoints.Add(new Vector2(x, y));
}
*/
    #endregion

}

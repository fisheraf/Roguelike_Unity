using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using System.Diagnostics;
using Debug = UnityEngine.Debug;

using NesScripts.Controls.PathFind;

public class GameMap : MonoBehaviour
{
    public Stopwatch timer;
    

    [SerializeField] Tilemap tilemap;
    [SerializeField] Engine engine;

    public Vector2Int botLeft;
    public Vector2Int size;

    [Tooltip("Needs to be multiple of 2")]
    public int mapWidth = 90;//need to be multiples of 2
    [Tooltip("Needs to be multiple of 2")]
    public int mapHeight = 50;

    public int maxRoomSize = 10;
    public int minRoomSize = 5;
    public int maxNumberOfRooms = 50;

    public int maxNumberOfMonstersPerRoom = 3;

    //public int fovAlgorithm = 0;
    public bool fovLightWalls = true;
    public int fovRadius = 8;

    bool fovRecompute = true;

    [SerializeField] TileBase wallTiles;
    [SerializeField] TileBase floorTiles;

    public Color wallColor;
    public Color wallColorLit;
    public Color floorColor;
    public Color floorColorLit;

    //[SerializeField] Vector2[] wallLocations; //set by script...
    //[SerializeField] Vector2[] floorLocations;

    //public RectInt r;

    public GameObject player;

    List<Vector3Int> seenTiles = new List<Vector3Int>();
    List<Vector3Int> recentTiles = new List<Vector3Int>();
    List<Vector3Int> previousTiles = new List<Vector3Int>();

    List<Vector3Int> visibleFloorTiles = new List<Vector3Int>();
    List<Vector3Int> visibleWallTiles = new List<Vector3Int>();
    List<Vector3Int> visibleTiles = new List<Vector3Int>();

    int numOfTiles;

    //List<bool> tileSeen = new List<bool>(4500); //remove hard code?
    //List<bool> tileVisible = new List<bool>(4500);
    //List<bool> tileIsWall = new List<bool>(4500);


    bool[,] tileSeen = new bool[96, 54];
    bool[,] tileVisible = new bool[96, 54];
    public bool[,] tileIsWall = new bool[96, 54];

    bool [,] hasEntity = new bool [96, 54];
    //bool is item

    public List<GameObject> entities = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        engine = FindObjectOfType<Engine>();
        //set camera size from width & height?
        //PlaceTile();        
        FillMap();
        numOfTiles = mapHeight * mapWidth;
        //MakeMap();
        //r = new RectInt(botLeft, size);
        //CreateRoom(r);        
        //PlaceTile(8, 8, wallTiles);
        player = GameObject.Find("Player");
        //FOV2();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            FillMap();
            MakeMap();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ShowMap();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            engine.CreateEntity((int)player.transform.position.x + 1, (int)player.transform.position.y + 1, -1 , 1, new Color32(255, 0, 0, 255), "AI", false, true);
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

    void FillMap()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                PlaceTile(i, j, wallTiles);
                //Debug.Log(i * mapHeight + j);
                tileIsWall[i, j] = true;
            }
        }
    }

    void MakeMap()
    {
        timer = new Stopwatch();
        timer.Start();

        List<Rect> rooms = new List<Rect>();
        int numberOfRooms = 0;

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
                RectInt newRoomInt = new RectInt(Mathf.RoundToInt(newRoom.xMin + 1), 
                    Mathf.RoundToInt(newRoom.yMin + 1),
                    Mathf.RoundToInt(newRoom.width - 2), 
                    Mathf.RoundToInt(newRoom.height - 2));
                //StartCoroutine(CreateRoomD(newRoomInt));
                CreateRoom(newRoomInt);

                //center of room for tunnels
                int newX = Mathf.RoundToInt(newRoomInt.center.x);
                int newY = Mathf.RoundToInt(newRoomInt.center.y);


                if (numberOfRooms == 0)
                {
                    engine.CreateEntity(newX, newY, -1, 0, new Color32(0, 135, 255, 255), "Player");
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

                PlaceEntities(newRoomInt);

                rooms.Add(newRoom);
                numberOfRooms++;
            }
            //yield return new WaitForSeconds(.1f);
        }
        timer.Stop();
        Debug.Log("Creating map took:" + timer.Elapsed);
        //FOVRecompute();
        FOV();
        //GetVisibleCells();
    }
    

    void CreateRoom(RectInt rectInt)
    {
        for (int i = rectInt.xMin; i < rectInt.xMax; i++)
        {
            for (int j = rectInt.yMin; j < rectInt.yMax; j++)
            {
                PlaceTile(i, j, floorTiles);
                tileIsWall[i, j] = false;
                //StartCoroutine(PlaceTileWithDelay(i, j, floorTiles));
            }
        }
        //PlaceEntities(rectInt);
    }
    void CreateHorizontalTunnel(int x1, int x2, int y)
    {
        for (int x = Mathf.Min(x1, x2); x < Mathf.Max(x1, x2) + 1; x++)
        {
            PlaceTile(x, y, floorTiles);
            tileIsWall[x, y] = false;
            //StartCoroutine(PlaceTileWithDelay(x, y, floorTiles));
        }
    }

    void CreateVerticalTunnel(int y1, int y2, int x)
    {
        for (int y = Mathf.Min(y1, y2); y < Mathf.Max(y1, y2) + 1; y++)
        {
            PlaceTile(x, y, floorTiles);
            tileIsWall[x, y] = false;

            //StartCoroutine(PlaceTileWithDelay(x, y, floorTiles));
        }
    }

    void PlaceTile(int x, int y, TileBase tile)
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

    int DiagDistance(int x, int y, int x1, int y1)
    {
        int dx = x1 - x;
        int dy = y1 - y;
        return  Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
    }

    public void FOV()
    {
        //Debug.Log("FOV");
        ClearVisibleTiles();
        timer = new Stopwatch();
        timer.Start();

        visibleWallTiles.Clear();
        visibleFloorTiles.Clear();

        previousTiles.AddRange(recentTiles);

        recentTiles.Clear();
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

                recentTiles.Add(new Vector3Int(tx, ty, 0));
            }
        }

        //RenderRecent();

        timer.Stop();
        //Debug.Log("Rendering took:" + timer.Elapsed);
        previousTiles.Clear();

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
         

    void Render()
    {
        ResetEntities();
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
                            if (entity.transform.position.x == x && entity.transform.position.y == y)
                            {
                                entity.transform.position = new Vector3(x, y, -1);
                            }
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
    }

    void ResetEntities()
    {
        foreach (GameObject entity in entities)
        {            
            {                
                entity.transform.position = new Vector3(entity.transform.position.x, entity.transform.position.y, 1);
                //entity.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }
        }
    }
       

    public void PlaceEntities(RectInt rectInt)
    {
        int numberOfMonsters = Random.Range(0, maxNumberOfMonstersPerRoom);

        for (int i = 0; i < numberOfMonsters; i++)
        {
            int x = Random.Range(rectInt.x, rectInt.x + rectInt.width);
            int y = Random.Range(rectInt.y, rectInt.y + rectInt.height);


            {
                if(Random.Range(0,100) < 20)
                {
                    engine.CreateEntity(x, y, 1, 1, new Color32(0, 80, 0, 255), "Orc");

                }
                else
                {
                    engine.CreateEntity(x, y, 1, 1, new Color32(0, 160, 0, 255), "Goblin");
                }
            }

        }
    }

    public void LocateEntities()
    {
        foreach(GameObject entity in entities)
        {
            //
        }
    }

    public float[,] tilesMap;
    public GridScript grid;

    void ConvertMap()
    {
        float[,] tilesMap = new float[mapWidth, mapHeight];
        for  (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                if (tileIsWall[i, j]) tilesMap[i, j] = 0f;

                else tilesMap[i, j] = 1f;
            }
                
        }

        grid = new GridScript(tilesMap);
    }


    #region Other FOV Attempts
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

    #endregion

}

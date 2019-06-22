using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMap : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;

    public Vector2Int botLeft;
    public Vector2Int size;

    [Tooltip("Needs to be multiple of 2")]
    public int mapWidth = 50;//need to be multiples of 2
    [Tooltip("Needs to be multiple of 2")]
    public int mapHeight = 90;

    public int maxRoomSize = 10;
    public int minRoomSize = 5;
    public int maxNumberOfRooms = 50;


    [SerializeField] TileBase wallTiles;
    [SerializeField] TileBase floorTiles;

    //[SerializeField] Vector2[] wallLocations; //set by script...
    //[SerializeField] Vector2[] floorLocations;

    //public RectInt r;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //set camera size from width & height?
        //PlaceTile();
        FillMap();
        //MakeMap();
        //r = new RectInt(botLeft, size);
        //CreateRoom(r);        
        //PlaceTile(8, 8, wallTiles);
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FillMap();
            MakeMap();
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
            }
        }
    }

    void MakeMap()
    {
        List<Rect> rooms = new List<Rect>();
        int numberOfRooms = 0;

        //level variety with random maxNumberOfRooms?

        for (int r = 0; r < maxNumberOfRooms; r++)
        {
            //random room size
            int w = Random.Range(minRoomSize, maxRoomSize);
            int h = Random.Range(minRoomSize, maxRoomSize);
            //random position inside map
            int x = Random.Range(0, mapWidth - w - 1);
            int y = Random.Range(0, mapHeight - h - 1);

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
                    player.transform.position = new Vector3(newX, newY, -1);
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

                rooms.Add(newRoom);
                numberOfRooms++;
            }
            //yield return new WaitForSeconds(.1f);
        }
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

    void CreateRoom(RectInt rectInt)
    {
        for (int i = rectInt.xMin; i < rectInt.xMax; i++)
        {
            for (int j = rectInt.yMin; j < rectInt.yMax; j++)
            {
                PlaceTile(i, j, floorTiles);                
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

    void CreateHorizontalTunnel(int x1, int x2, int y)
    {
        for (int x = Mathf.Min(x1, x2); x < Mathf.Max(x1, x2) + 1; x++)
        {
            PlaceTile(x, y, floorTiles);            
            //StartCoroutine(PlaceTileWithDelay(x, y, floorTiles));
        }
    }

    void CreateVerticalTunnel(int y1, int y2, int x)
    {
        for (int y = Mathf.Min(y1, y2); y < Mathf.Max(y1, y2) + 1; y++)
        {
            PlaceTile(x, y, floorTiles);
            
            //StartCoroutine(PlaceTileWithDelay(x, y, floorTiles));
        }
    }


    void PlaceTile(int x, int y, TileBase tile)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    IEnumerator PlaceTileWithDelay(int x, int y, TileBase tile)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
        yield return new WaitForSeconds(1);
    }
}

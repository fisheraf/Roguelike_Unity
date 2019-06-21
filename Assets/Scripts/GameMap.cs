using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public int width;
    public int height;

    [SerializeField] GameObject wallTiles;
    [SerializeField] GameObject floorTiles;

    [SerializeField] Vector2[] wallLocations; //set by script...
    [SerializeField] Vector2[] floorLocations;


    // Start is called before the first frame update
    void Start()
    {
        //set camera size from width & height?
        //PlaceTile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceTile()// X & Y?
    {
        for (int i = 0; i < wallLocations.Length; i++)
        {
            Instantiate(wallTiles, wallLocations[i], Quaternion.identity);
        }
        for (int i = 0; i < floorLocations.Length; i++)
        {
            Instantiate(floorTiles, floorLocations[i], Quaternion.identity);
        }
    }
}

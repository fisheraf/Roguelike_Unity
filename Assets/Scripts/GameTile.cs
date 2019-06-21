using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEngine.Tilemaps
{    
    public enum TileType { floor, wall}
    public class GameTile : Tile
    {
        TileType tileType = TileType.floor;
        public bool isBlocked;
        
        // Start is called before the first frame update
        void Start()
        {
            if(tileType == TileType.floor)
            {
                isBlocked = false;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

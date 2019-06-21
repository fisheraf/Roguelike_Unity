using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isBlocked { get; set; }
    public bool isblocked;
    public bool sightBlocked { get; set; }

    public int xPosition { get; set; }
    public int yPosition { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        if (isBlocked)
        {
            sightBlocked = false;
        }

        xPosition = Mathf.RoundToInt(transform.position.x);
        yPosition = Mathf.RoundToInt(transform.position.y);

    }

    // Update is called once per frame
    void Update()
    {

    }
}



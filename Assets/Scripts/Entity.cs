using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{    
    public Vector2 position;//seperate to X & Y?
    public string entityName;
    public Sprite entitySprite;

    SpriteRenderer spriteRenderer;

    [SerializeField] public CircleCollider2D[] directionalColliders;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        gameObject.name = entityName;
        spriteRenderer.sprite = entitySprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(int dx, int dy)
    {
        //following keypad movement (1-9 order this time)
        //down left
        if(dx == -1 && dy == -1)
        {
            gameObject.transform.position -= transform.up + transform.right;
            //position += dx & dy
        }
        //down
        if (dx == 0 && dy == -1)
        {
            gameObject.transform.position -= transform.up;
        }
        //down right
        if (dx == 1 && dy == -1)
        {
            gameObject.transform.position -= transform.up - transform.right;
        }
        //left
        if (dx == -1 && dy == 0)
        {
            gameObject.transform.position -= transform.right;
        }
        //wait
        if (dx == 0 && dy == 0)
        {
            //skip turn
        }
        //right
        if (dx == 1 && dy == 0)
        {
            gameObject.transform.position += transform.right;
        }
        //up left
        if (dx == -1 && dy == 1)
        {
            gameObject.transform.position += transform.up - transform.right;
        }
        //up
        if (dx == 0 && dy == 1)
        {
            gameObject.transform.position += transform.up;
        }
        //up right
        if (dx == 1 && dy == 1)
        {
            gameObject.transform.position += transform.up + transform.right;
        }

    }
}

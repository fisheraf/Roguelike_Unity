using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Entity : MonoBehaviour
{    
    public Vector2 position;//seperate to X & Y?
    public int entityNumber;
    //public string entityName;
    public Sprite[] entitySprites;

    SpriteRenderer spriteRenderer;

    TextMeshPro text;

    public Fighter fighter;
    //public AI ai;

    public bool hasActed = true;
    public bool isDead = false;

    public bool isConfused = false;
    public int confusedTurnsLeft = 0;

    //[SerializeField] public CircleCollider2D[] directionalColliders;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        text = GetComponentInChildren<TextMeshPro>();
        //gameObject.name = entityName;
        //spriteRenderer.sprite = entitySprites[0];

    }

    // Update is called once per frame
    void Update()
    {
        position.x = transform.position.x;
        position.y = transform.position.y;

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

    public void KillEntity()
    {
        //spriteRenderer.sprite = entitySprites[1];
        //spriteRenderer.color = Color.red;
        text.text = "x";
        text.color = Color.red;

        name = "Dead " + name;
        FindObjectOfType<GameMap>().entities.Remove(gameObject);
        FindObjectOfType<GameMap>().deadEntities.Add(gameObject);
        isDead = true;
    }
}

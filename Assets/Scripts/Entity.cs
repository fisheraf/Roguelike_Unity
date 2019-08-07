using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Entity : MonoBehaviour
{    
    public int entityNumber;
    public Sprite[] entitySprites;

    SpriteRenderer spriteRenderer = null;
    TextMeshPro text;

    public Fighter fighter;

    public bool hasActed = true;
    public bool isDead = false;

    public bool isConfused = false;
    public int confusedTurnsLeft = 0;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        text = GetComponentInChildren<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {

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

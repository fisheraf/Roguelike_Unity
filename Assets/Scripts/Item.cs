using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Vector2 position;
    public Sprite[] entitySprites;
    SpriteRenderer spriteRenderer;

    int value;

    Engine engine = null;
    GameMap gameMap = null;
    UIManager uiManager = null;
    Player player = null;
    Inventory inventory = null;

    public enum ItemType { Null, HealingPotion, LightningScroll, Fireball, ConfusionScroll }

    IDictionary<int, string> itemDictionary = new Dictionary<int, string>()
    {
        {1, "Healing Potion" },
        {2, "Lightning Scroll" },
        {3, "Fireball Scroll" },
        {4, "Confusion Scroll" }
    };

    ItemType itemType = ItemType.Null;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        engine = FindObjectOfType<Engine>();
        gameMap = FindObjectOfType<GameMap>();
        uiManager = FindObjectOfType<UIManager>();
        player = FindObjectOfType<Player>();
        inventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = FindObjectOfType<Player>();
        }
        position.x = transform.position.x;
        position.y = transform.position.y;
    }
    
    /*
    public void SetItem(ItemType type, int number)
    {
        itemType = type;
        value = number;

        if(type == ItemType.HealingPotion)
        {
            gameObject.name = "Healing Potion";
            spriteRenderer.sprite = entitySprites[0];
            spriteRenderer.color = Color.red;
        }
        if(type == ItemType.LightningScroll)
        {
            gameObject.name = "Lightning Scroll";
            spriteRenderer.sprite = entitySprites[1];
            spriteRenderer.color = Color.yellow;
        }
        if (type == ItemType.Fireball)
        {
            gameObject.name = "Fireball Scroll";
            spriteRenderer.sprite = entitySprites[1];
            spriteRenderer.color = Color.red;
        }
        if (type == ItemType.ConfusionScroll)
        {
            gameObject.name = "Confusion Scroll";
            spriteRenderer.sprite = entitySprites[1];
            spriteRenderer.color = Color.magenta;
        }
    }*/
    
    public void SetItem(int itemNumber)
    {
        //itemType = type;
        //value = number;

        if (itemNumber == 1)
        {
            gameObject.name = "Healing Potion";
            spriteRenderer.sprite = entitySprites[0];
            spriteRenderer.color = Color.red;
            itemType = ItemType.HealingPotion;
        }
        if (itemNumber == 2)
        {
            gameObject.name = "Lightning Scroll";
            spriteRenderer.sprite = entitySprites[1];
            spriteRenderer.color = Color.yellow;
            itemType = ItemType.LightningScroll;
        }
        if (itemNumber == 3)
        {
            gameObject.name = "Fireball Scroll";
            spriteRenderer.sprite = entitySprites[1];
            spriteRenderer.color = Color.red;
            itemType = ItemType.Fireball;
        }
        if (itemNumber == 4)
        {
            gameObject.name = "Confusion Scroll";
            spriteRenderer.sprite = entitySprites[1];
            spriteRenderer.color = Color.magenta;
            itemType = ItemType.ConfusionScroll;
        }
    }
    

    public void PickUpObject()
    {
        gameMap.items.Remove(gameObject);
        uiManager.NewMessage("You pick up " + name + ".");
        inventory.items.Add(this.gameObject);
        //gameObject.SetActive(false);
        gameObject.transform.position = new Vector3(-10, -10, -1);
    }

    public void UseObject()
    {
        if(itemType == ItemType.HealingPotion)
        {
            Fighter fighter = player.gameObject.GetComponent<Fighter>();
            if(fighter.HP == fighter.MaxHP)
            {
                //Debug.Log("full HP");
                uiManager.NewMessage("You are already at full health.");
                return;
            }
            else
            {
                //Debug.Log("Heal");
                int healthGained = 0;
                value = 10;
                if(fighter.MaxHP-fighter.HP < value)
                {
                    healthGained = (fighter.MaxHP - fighter.HP);
                }
                else
                {
                    healthGained = value;
                }
                uiManager.NewMessage("You gain <size=200%><voffset=-.2em ><#023788>" + healthGained + "</size></voffset></color> health back from the potion.");
                Heal();

                ItemUsed();
            }
        }

        if(itemType == ItemType.LightningScroll)
        {
            GameObject target = null;
            int closestDistance = 7;
            foreach (GameObject entity in engine.entities)
            {
                if (gameMap.tileVisible[(int)entity.transform.position.x, (int)entity.transform.position.y])
                {
                    int distanceToEntity = gameMap.DiagDistance((int)player.transform.position.x, (int)player.transform.position.y, (int)entity.transform.position.x, (int)entity.transform.position.y);
                    if(distanceToEntity < closestDistance)
                    {
                        target = entity;
                        closestDistance = distanceToEntity;
                    }
                }
            }
            if (target != null)
            {
                uiManager.NewMessage("You strike " + target.name + " with a bolt of lightning");
                target.GetComponent<Fighter>().takeDamage(value);

                ItemUsed();
            }
            else
            {
                uiManager.NewMessage("No targets in range.");
                return;
            }
        }

        if (itemType == ItemType.Fireball)
        {
            gameObject.SetActive(true);
            gameObject.transform.position = new Vector3(-10, -10, -1);

            StartCoroutine(Targeting(false));
        }

        if(itemType == ItemType.ConfusionScroll)
        {
            gameObject.SetActive(true);
            gameObject.transform.position = new Vector3(-10, -10, -1);

            StartCoroutine(Targeting(true));
        }


    }

    public void ItemUsed()
    {
        inventory.items.Remove(this.gameObject);
        inventory.CloseInventory();
        engine.gameState = Engine.GameState.EnemyTurn;
    }


    IEnumerator Targeting(bool entityRequired)
    {
        StartCoroutine(engine.TargetSelect(entityRequired));
        //yield return new WaitUntil(() => engine.targetSelected == true);
        while(engine.targetSelected == false)
        {
            yield return null;
        }
        //yield return StartCoroutine(engine.WaitForTargetSelection());

        if (itemType == ItemType.Fireball)
        {
            Fireball(engine.target);
        }
        if (itemType == ItemType.ConfusionScroll)
        {
            Confusion(engine.target);
        }

    }


    void Fireball(Vector2Int target)
    {
        //Debug.Log("Fireball cast at " + target);
        int radius = 3;

        //target = engine.target;
        

        for (int i = engine.entities.Count - 1; i >= 0; i--)
        {
            int distanceToEntity = gameMap.DiagDistance(target.x, target.y, (int)engine.entities[i].transform.position.x, (int)engine.entities[i].transform.position.y);
            //Debug.Log(i + "," + distanceToEntity);
            if (distanceToEntity <= radius)
            {
                uiManager.NewMessage("The " + engine.entities[i].name + " is engulfed in flames.");
                engine.entities[i].GetComponent<Fighter>().takeDamage(value);

                //damage to player if in range
            }
        }

        if(gameMap.DiagDistance(target.x, target.y, (int)player.transform.position.x, (int)player.transform.position.y) <= radius)
        {
            uiManager.NewMessage("The " + player.name + " is engulfed in flames.");
            player.gameObject.GetComponent<Fighter>().takeDamage(value);
        }

        inventory.items.Remove(this.gameObject);
        gameObject.SetActive(false);
        engine.gameState = Engine.GameState.EnemyTurn;
    }

    void Confusion(Vector2Int target)
    {
        foreach(GameObject entity in engine.entities)
        {
            if((target.x == (int)entity.transform.position.x) && (target.y == (int)entity.transform.position.y))
            {
                uiManager.NewMessage("The " + entity.name + " becomes confused.");
                entity.GetComponent<Entity>().isConfused = true;
                entity.GetComponent<Entity>().confusedTurnsLeft = value;
            }
        }

        inventory.items.Remove(this.gameObject);
        gameObject.SetActive(false);
        engine.gameState = Engine.GameState.EnemyTurn;
    }
    

    public void DropObject()
    {
        gameMap.items.Add(gameObject);
        inventory.items.Remove(this.gameObject);

        if(gameObject.GetComponent<Equippable>() !=false)
        {
            gameObject.GetComponent<Equippable>().equipped = false;
        }

        transform.position = player.transform.position;

        gameObject.SetActive(true);

        uiManager.NewMessage("You drop the " + name + " on the floor.");

        inventory.CloseInventory();
        engine.gameState = Engine.GameState.EnemyTurn;
    }

    void Heal()
    {
        FindObjectOfType<Player>().GetComponent<Fighter>().heal(value);
    }
}

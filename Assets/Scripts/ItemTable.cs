using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemTable : MonoBehaviour
{
    Engine engine;

    private void Awake()
    {
        engine = FindObjectOfType<Engine>();
    }


    public List<KeyValuePair<int, int>> itemCountTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(1,1),
        new KeyValuePair<int, int>(2,4),
        new KeyValuePair<int, int>(3,6)
    };



    public Dictionary<int, string> itemDict = new Dictionary<int, string>
    {
        {10000, "nano" },
        {500, "healing potion" },
        {0, "lightning scroll" },
        {1, "fireball scroll" },
        {2, "confusion scroll" },
        {3, "sword" },
        {4, "shield" }

    };


    //likelihood of item (key) after level (value)
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
        //new KeyValuePair<int, int>(3,0),
        new KeyValuePair<int, int>(101,2)
    };

    List<KeyValuePair<int, int>> shieldChanceTable = new List<KeyValuePair<int, int>>()
    {
        //new KeyValuePair<int, int>(4,0),
        new KeyValuePair<int, int>(102,2)
    };



    public void UpdateDicts()
    {
        itemDict.Clear();
        itemDict.Add(FromDungeonLevel(healingChanceTable), "healing potion");
        itemDict.Add(FromDungeonLevel(lightningChanceTable), "lightning scroll");
        itemDict.Add(FromDungeonLevel(fireballChanceTable), "fireball scroll");
        itemDict.Add(FromDungeonLevel(confusionChanceTable), "confusion scroll");
        if(engine.dungeonLevel >1)
        {
            itemDict.Add(FromDungeonLevel(swordChanceTable), "sword");
            itemDict.Add(FromDungeonLevel(shieldChanceTable), "shield");
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

    public void PlaceItem(int x, int y)
    {
        string itemChoice = RandomChoiceFromDict(itemDict);

        if (itemChoice == "healing potion")
        {
            engine.CreateItem(x, y, 0);
        }
        else if (itemChoice == "lightning scroll")
        {
            engine.CreateItem(x, y, 1);
        }
        else if (itemChoice == "fireball scroll")
        {
            engine.CreateItem(x, y, 2);
        }
        else if (itemChoice == "confusion scroll")
        {
            engine.CreateItem(x, y, 3);
        }

        else if (itemChoice == "sword")
        {
            engine.CreateItem(x, y, 4);
        }
        else if (itemChoice == "shield")
        {
            engine.CreateItem(x, y, 5);
        }

        else if (itemChoice == "nano")
        {
            engine.CreateItem(x, y, 6);
        }
    }
}


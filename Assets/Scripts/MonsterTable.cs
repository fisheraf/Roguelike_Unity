using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterTable : MonoBehaviour
{
    Engine engine;

    private void Awake()
    {
        engine = FindObjectOfType<Engine>();
    }


    //numer of monsters (key) after level (value)
    public List<KeyValuePair<int, int>> monsterCountTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(2,1),
        new KeyValuePair<int, int>(3,4),
        new KeyValuePair<int, int>(5,6)
    };


    public Dictionary<int, string> monsterDict = new Dictionary<int, string>
    {
        {900, "goblin" },
        {100, "orc" }
    };


    public Dictionary<int, string> itemDict = new Dictionary<int, string>
    {

        {500, "healing potion" },
        {0, "lightning scroll" },
        {1, "fireball scroll" },
        {2, "confusion scroll" },
        {3, "sword" },
        {4, "shield" }

    };


    List<KeyValuePair<int, int>> goblinChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(900,0)
    };
    List<KeyValuePair<int, int>> orcChanceTable = new List<KeyValuePair<int, int>>()
    {
        new KeyValuePair<int, int>(151,3),
        new KeyValuePair<int, int>(301,5),
        new KeyValuePair<int, int>(601,7)
    };



    public void UpdateDicts()
    {
        monsterDict.Clear();
        monsterDict.Add(FromDungeonLevel(goblinChanceTable), "goblin");
        monsterDict.Add(FromDungeonLevel(orcChanceTable), "orc");
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

    public void PlaceMonster(int x, int y)
    {
        string monsterChoice = RandomChoiceFromDict(monsterDict);
        if (monsterChoice == "orc")
        {
            engine.CreateEntity(x, y, 1);
        }
        else
        {
            engine.CreateEntity(x, y, 0);
        }
    }
}

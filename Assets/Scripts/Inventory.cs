using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Inventory : MonoBehaviour
{
    Player player = null;
    GameMap gameMap = null;
    GameObject eventSystem = null;
    Engine engine = null;
    public List<GameObject> items = new List<GameObject>();
    public List<int> itemIDNumber = new List<int>();

    GameObject panel;
    [SerializeField] Button[] button = null;
    //[SerializeField] TextMeshProUGUI[] itemText = null;

    Equipment equipment = null;
    UIManager uIManager = null;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        gameMap = FindObjectOfType<GameMap>();
        eventSystem = GameObject.Find("EventSystem");
        engine = FindObjectOfType<Engine>();
        equipment = FindObjectOfType<Equipment>();
        panel = GameObject.Find("InventoryPanel");
        panel.SetActive(false);
        uIManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenInventory()
    {
        panel.SetActive(true);
        UpdateItemText();
    }

    public void CloseInventory()
    {
        panel.SetActive(false);
        UpdatePlayerStats();
    }

    void UpdateItemText()
    {
        equipment.equipmentList.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                button[i].gameObject.SetActive(true);
                button[i].GetComponentInChildren<TextMeshProUGUI>().text = items[i].name;                
            }
            if(items[i].GetComponent<Equippable>() != null)
            {
                equipment.equipmentList.Add(items[i].GetComponent<Equippable>());

                if (items[i].GetComponent<Equippable>().equipped)
                {
                    button[i].GetComponentInChildren<TextMeshProUGUI>().text = items[i].GetComponent<Equippable>().name + "(equipped in" + items[i].GetComponent<Equippable>().slot + ")";
                }
                else
                {
                    button[i].GetComponentInChildren<TextMeshProUGUI>().text = items[i].GetComponent<Equippable>().name;
                }
            }

        }
        for (int i = items.Count; i < button.Length; i++)
        {
            button[i].gameObject.SetActive(false);
        }


        EventSystem.current.firstSelectedGameObject = button[0].gameObject;
        EventSystem.current.SetSelectedGameObject(button[0].gameObject);
    }

    public void UseItem(int itemNumber)
    {
        if (engine.gameState == Engine.GameState.ShowInventory)
        {
            if (items[itemNumber] != null)
            {
                if (items[itemNumber].GetComponent<Equippable>() != null)
                {
                    Debug.Log("equipped");
                    items[itemNumber].GetComponent<Equippable>().ToggleEquip();
                }
                else
                {

                    Debug.Log("use item 0");
                    items[itemNumber].GetComponent<Item>().UseObject();
                }
            }
            else
            {
                Debug.Log("no item");
                //add to meassage system
            }
        }
        else if(engine.gameState == Engine.GameState.DropInventory)
        {
            if (items[itemNumber] != null)
            {
                items[itemNumber].GetComponent<Item>().DropObject();
            }
            else
            {
                Debug.Log("no item to drop");
                //add to meassage system
            }
        }

    }

    Fighter playerfighter;

    public void UpdatePlayerStats()
    {
        if(player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if( playerfighter == null)
        {
            playerfighter = player.GetComponent<Fighter>();
        }

        playerfighter.bonusMaxHP = equipment.HPBonus();
        playerfighter.bonusDefense = equipment.DefenseBonus();
        playerfighter.bonusPower = equipment.PowerBonus();

        uIManager.SetUIText();
    }

    public void LoadItems()
    {
        items.Clear();
        for (int i = 0; i < itemIDNumber.Count; i++)
        {
            int x = itemIDNumber[i];

            if(x == 1)
            {
                GameObject itemObject = Instantiate(engine.itemObject, new Vector3(-10, -10, -1), Quaternion.identity);
                itemObject.GetComponent<Item>().SetItem(1);// itemType, value);
                
                items.Add(itemObject);
                itemObject.SetActive(false);
            }
            if (x == 2)
            {
                GameObject itemObject = Instantiate(engine.itemObject, new Vector3(-10, -10, -1), Quaternion.identity);
                itemObject.GetComponent<Item>().SetItem(2);// itemType, value);

                items.Add(itemObject);
                itemObject.SetActive(false);
            }
            if (x == 3)
            {
                GameObject itemObject = Instantiate(engine.itemObject, new Vector3(-10, -10, -1), Quaternion.identity);
                itemObject.GetComponent<Item>().SetItem(3);// itemType, value);

                items.Add(itemObject);
                itemObject.SetActive(false);
            }
            if (x == 4)
            {
                GameObject itemObject = Instantiate(engine.itemObject, new Vector3(-10, -10, -1), Quaternion.identity);
                itemObject.GetComponent<Item>().SetItem(4);//itemType, value);

                items.Add(itemObject);
                itemObject.SetActive(false);
            }
            if (x == 101)
            {
                GameObject itemObject = Instantiate(engine.equipmentObject, new Vector3(-10, -10, -1), Quaternion.identity);
                itemObject.GetComponent<Equippable>().SetEquipment(101);//itemType, value);

                items.Add(itemObject);
                equipment.equipmentList.Add(itemObject.GetComponent<Equippable>());
                //itemObject.SetActive(false);
            }
            if (x == 102)
            {
                GameObject itemObject = Instantiate(engine.equipmentObject, new Vector3(-10, -10, -1), Quaternion.identity);
                itemObject.GetComponent<Equippable>().SetEquipment(102);//itemType, value);

                items.Add(itemObject);
                equipment.equipmentList.Add(itemObject.GetComponent<Equippable>());
                //itemObject.SetActive(false);
            }

        }
    }

    public void SaveItems()
    {
        itemIDNumber.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            if(items[i].name == "Healing Potion")
            {
                itemIDNumber.Add(1);
            }
            if (items[i].name == "Lightning Scroll")
            {
                itemIDNumber.Add(2);
            }
            if (items[i].name == "Fireball Scroll")
            {
                itemIDNumber.Add(3);
            }
            if (items[i].name == "Confusion Scroll")
            {
                itemIDNumber.Add(4);
            }
            if (items[i].name == "Sword")
            {
                itemIDNumber.Add(101);
            }
            if (items[i].name == "Shield")
            {
                itemIDNumber.Add(102);
            }

        }
    }
}

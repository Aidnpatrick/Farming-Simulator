using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public struct Item
{
    public string name;
    public int price;
    public string description;

    public Item(string name, int price, string description)
    {
        this.name = name;
        this.price = price;
        this.description = description;
    }
}

public class StandScript : MonoBehaviour
{
    private GameObject player;

    public GameObject imgGUI;
    public GameObject standGUI;
    public GameObject headingStandGUI;
    public InventoryScript inventoryScript;

    public bool isActive = false;

    private List<Item> items = new List<Item>();

    void Start()
    {
        player = GameObject.Find("Player");

        items.Add(new Item("Hoe", 6, "It creates dirt which lets you plant seeds."));
        items.Add(new Item("ChestPlacer", 3, "Lets you store multiple items easily."));
        items.Add(new Item("Seeds", 2, "Place seeds into the dirt and sell the plant for profit."));
        items.Add(new Item("Materials", 2, "For the currency for placing and building things."));
        items.Add(new Item("TakeDown", 20, "Its a light rifle that is good for hunting."));
        UploadShop();

        standGUI.SetActive(false);
        headingStandGUI.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard.digit1Key.wasPressedThisFrame)
            BuyItem(0);
        if (keyboard.digit2Key.wasPressedThisFrame)
            BuyItem(1);
        if (keyboard.digit3Key.wasPressedThisFrame)
            BuyItem(2);
        if (keyboard.digit4Key.wasPressedThisFrame)
            BuyItem(3);
        if (keyboard.digit5Key.wasPressedThisFrame)
            BuyItem(4);
        if (keyboard.digit6Key.wasPressedThisFrame)
            BuyItem(5);
        if (keyboard.digit7Key.wasPressedThisFrame)
            BuyItem(6);
        if (keyboard.digit8Key.wasPressedThisFrame)
            BuyItem(7);
        if (keyboard.digit9Key.wasPressedThisFrame)
            BuyItem(8);
    }

    public void SetActive(bool value)
    {
        Debug.Log("SetActive called: " + value);

        if (isActive == value) return;

        isActive = value;

        standGUI.SetActive(isActive);
        headingStandGUI.SetActive(isActive);

        /*if (isActive)
        {
            UploadShop();
            EventSystem.current.SetSelectedGameObject(null);
        }
        */
    }

    void BuyItem(int index)
    {
        if (index < 0 || index >= items.Count) return;
        
        if(inventoryScript.coins < items[index].price)
        {
            Debug.Log("Not enough coins");
            return;
        }

        GetItem(items[index].name, items[index].price);
    }

    public void GetItem(string itemName, int numcoins)
    {
        if(itemName == "Material")
        {
            inventoryScript.materials+=10;
            inventoryScript.coins -= numcoins;
            return;
        }
        GameObject lootPrefab = Resources.Load<GameObject>(itemName + "Loot");
        if (lootPrefab == null) return;
        GameObject dropped = Instantiate(
            lootPrefab,
            transform.position + new Vector3(3,0,0),
            Quaternion.identity
        );

        dropped.tag = "Loot";
        dropped.name = itemName + "Loot";
        inventoryScript.coins -= numcoins;
    }

    public void UploadShop()
    {
        StartCoroutine(ClearSelectionNextFrame());
        foreach (Transform child in standGUI.transform)
            Destroy(child.gameObject);
        int index = 1;
        foreach (Item item in items)
        {
            GameObject clone = Instantiate(imgGUI, standGUI.transform, false);

            clone.transform.GetChild(0).GetComponent<TMP_Text>().text = item.name;
            clone.transform.GetChild(1).GetComponent<TMP_Text>().text = item.price.ToString();

            Button buyButton = clone.transform.GetChild(2).GetComponent<Button>();
            buyButton.onClick.RemoveAllListeners();

            Item capturedItem = item;
            buyButton.name = "Button (" + index + ")";
            buyButton.navigation = new Navigation { mode = Navigation.Mode.None };
            EventSystem.current.SetSelectedGameObject(null);
            index++;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void UploadEbay()
    {
        
    }

    /*public void Clicked(Item item)
    {
        Debug.Log("Clicked " + item.name);
        GetItem(item.name, item.price);
    }
    */
    IEnumerator ClearSelectionNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Contains("Tile"))
        {
            Destroy(collision.gameObject);
        }
    }
}

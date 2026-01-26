using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using JetBrains.Annotations;
using NUnit.Framework;

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

    public GameObject imgGUI, sellImgGUI;
    public GameObject standGUI;
    public GameObject headingStandGUI;
    private InventoryScript inventoryScript;

    public bool isActive = false, isBuying = true;

    private List<Item> items = new List<Item>();

    void Start()
    {
        inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
        player = GameObject.Find("Player");
        

        items.Add(new Item("Hoe", 6, "It creates dirt which lets you plant seeds."));
        items.Add(new Item("Shovel", 8, "It makes Gravel Path which makes player faster."));
        items.Add(new Item("ChestPlacer", 3, "Lets you store multiple items easily."));
        items.Add(new Item("Seeds", 2, "Place seeds into the dirt and sell the plant for profit."));
        items.Add(new Item("FencePlacer", 5, "A simple building block. Good for defense."));
        items.Add(new Item("Materials", 2, "For the currency for placing and building things."));
        items.Add(new Item("TakeDown", 20, "Its a light rifle that is good for hunting."));
        standGUI.SetActive(false);
        headingStandGUI.SetActive(false);
        transform.position += new Vector3(1,0,0);
    }

    void Update()
    {
        if (!isActive) return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard.digit1Key.wasPressedThisFrame && isBuying == true)
        {
            BuyItem(0);            
        }
        else if(keyboard.digit1Key.wasPressedThisFrame && isBuying == false)
        {
            SellItem();
        }
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
        if(isBuying) UploadShop();
        else UploadSell();
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
    public void GetItem(string itemName, int numcoins = 0)
    {
        if(itemName == "Materials")
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

    public void SellItem()
    {
        int sum = 0;
        foreach(Stock i in inventoryScript.stocks)
        {
            sum += i.price * i.quantity;
        }
        inventoryScript.coins += sum;
        inventoryScript.stocks.Clear();
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

            clone.transform.GetChild(0).GetComponent<TMP_Text>().text = item.name + "\n#" + index;

            Image img = clone.transform.GetChild(1).GetComponent<Image>();
            img.sprite = Resources.Load<Sprite>("Images/" + item.name);

            clone.transform.GetChild(2).GetComponent<TMP_Text>().text = item.price.ToString();

            Button buyButton = clone.GetComponentInChildren<Button>();
            buyButton.onClick.RemoveAllListeners();

            Item capturedItem = item;
            buyButton.onClick.AddListener(() => BuyItem(capturedItem.price));

            buyButton.navigation = new Navigation { mode = Navigation.Mode.None };
        }
        headingStandGUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Walstand";    
        headingStandGUI.GetComponent<Image>().color = new Color32(0, 83, 226, 255);
        EventSystem.current.SetSelectedGameObject(null);
        index++;
    }
    public void UploadSell()
    {
        StartCoroutine(ClearSelectionNextFrame());
        foreach(Transform child in standGUI.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject clone = Instantiate(sellImgGUI, standGUI.transform, false);
        Button sellButton = clone.transform.GetComponentInChildren<Button>();
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => SellItem());

        headingStandGUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Selling Stand";
        headingStandGUI.GetComponent<Image>().color = new Color32(163 ,68, 27, 255);

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Contains("Tile"))
        {
            Destroy(collision.gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.name.Contains("Tile"))
        {
            Destroy(collision.gameObject);
        }
    }

}

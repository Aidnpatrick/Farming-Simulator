using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class Stock
{
    public string itemName;
    public int quantity;
    public int price;

    public Stock(string itemName, int quantity, int price)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.price = price;
    }
}

public class InventoryScript : MonoBehaviour
{
    public PlayerScript playerScript;
    public CameraScript cameraScript;
    public GameObject gameCanvas;
    public GameObject healthContainer;
    public GameObject tabContainer;
    public GameObject tabUI;
    public GameObject tabChild;
    public GameObject currentSlotImg;
    public TMP_Text materialText;
    public GameObject image;
    public GameObject heartPrefab;
    public GameObject player;

    public List<string> inventory = new List<string>();
    public List<Stock> stocks = new List<Stock>();

    public int equippedItem = 1;
    public int ammo = 10;
    public int materials = 1000;
    public int coins = 1000;
    private int maxInventorySize = 5;

    void Start()
    {
        UpdateInventory();
        UpdatePlayerChildren();
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.digit1Key.wasPressedThisFrame && inventory.Count > 0) equippedItem = 1;
        else if (keyboard.digit2Key.wasPressedThisFrame && inventory.Count > 1) equippedItem = 2;
        else if (keyboard.digit3Key.wasPressedThisFrame && inventory.Count > 2) equippedItem = 3;
        else if (keyboard.digit4Key.wasPressedThisFrame && inventory.Count > 3) equippedItem = 4;
        else if (keyboard.digit5Key.wasPressedThisFrame && inventory.Count > 4) equippedItem = 5;

        for (int i = 0; i < inventory.Count; i++)
        {
            Transform t = player.transform.Find(inventory[i] + (i + 1));
            if (t == null) continue;

            bool equipped = i == equippedItem - 1;
            t.gameObject.SetActive(equipped);

            if (equipped)
            {
                t.localPosition = Vector3.zero + new Vector3(0.5f,0,0);
                t.localRotation = Quaternion.identity;
                if(inventory.Count > 0)
                {
                    if(inventory[equippedItem - 1].Contains("TakeDown"))
                        cameraScript.canEdit = false;
                    else
                        cameraScript.canEdit = true;
                }
                else
                    cameraScript.canEdit = true;
            }
            else
                cameraScript.canEdit = true;

        }

        if (keyboard.qKey.wasPressedThisFrame)
            DropEquippedItem();

        materialText.text = "[Still in development]\nMaterials: " + materials + "\nAmmo: " + ammo + "\n$" + coins;

        int slotIndex = equippedItem - 1;

        if (slotIndex >= 0 && slotIndex < gameCanvas.transform.childCount)
        {
            currentSlotImg.SetActive(true);
            currentSlotImg.transform.position =
                gameCanvas.transform.GetChild(slotIndex).position;
        }
        else
        {
            currentSlotImg.SetActive(false);
        }



        UpdateHealth();

        tabUI.SetActive(keyboard.tabKey.isPressed);
        UpdateTabUI();
    }

    public void UpdatePlayerChildren()
    {
        foreach (Transform child in player.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject prefab = Resources.Load<GameObject>(inventory[i]);
            if (prefab == null) continue;

            GameObject item = Instantiate(prefab, player.transform);
            item.name = inventory[i] + (i + 1);
            item.transform.localPosition = Vector3.zero + new Vector3(0.5f,0,0);
            item.transform.localRotation = Quaternion.identity;
            item.SetActive(false);
        }
    }

    public void UpdateInventory()
    {
        foreach (Transform img in gameCanvas.transform)
            Destroy(img.gameObject);

        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject item = Instantiate(image, gameCanvas.transform, false);
            var imgComp = item.GetComponent<UnityEngine.UI.Image>();
            Sprite sprite = Resources.Load<Sprite>("Images/" + inventory[i]);
            if (sprite != null) imgComp.sprite = sprite;
        }
    }

    public void UpdateHealth()
    {
        foreach (Transform img in healthContainer.transform)
            Destroy(img.gameObject);

        for (int i = 0; i < playerScript.health; i++)
            Instantiate(heartPrefab, healthContainer.transform, false);
    }

    public void UpdateTabUI()
    {
        foreach (Transform img in tabContainer.transform)
            Destroy(img.gameObject);

        for (int i = 0; i < stocks.Count; i++)
        {
            GameObject c = Instantiate(tabChild, tabContainer.transform, false);
            c.transform.GetChild(0).GetComponent<TMP_Text>().text =
                stocks[i].itemName + " " + stocks[i].quantity;
        }
    }

    public void AddItem(string itemName)
    {
        Debug.Log(itemName);
        if (itemName.Contains("Ammo"))
        {
            ammo += 10;
            return;        
        }
        if (inventory.Count >= maxInventorySize) return;
        if (Resources.Load<GameObject>(itemName) == null) return;
        inventory.Add(itemName);
        equippedItem = Mathf.Clamp(equippedItem, 1, inventory.Count);
        UpdateInventory();
        UpdatePlayerChildren();
    }

    public void AddStock(string itemName, int amount, int price)
    {
        foreach (Stock s in stocks)
        {
            if (s.itemName == itemName)
            {
                s.quantity += amount;
                return;
            }
        }
        stocks.Add(new Stock(itemName, amount, price));
    }

    public void DropEquippedItem(bool isBlock = false)
    {
        if (inventory.Count == 0) return;

        int index = equippedItem - 1;
        if (index < 0 || index >= inventory.Count) return;

        string itemName = inventory[index];

        GameObject lootPrefab = Resources.Load<GameObject>(itemName + "Loot");
        if (lootPrefab != null && !isBlock)
        {
            GameObject dropped = Instantiate(lootPrefab, player.transform.position, Quaternion.identity);
            dropped.tag = "Loot";
            dropped.name = itemName + "Loot";
        }

        inventory.RemoveAt(index);
        equippedItem = inventory.Count == 0 ? 1 : Mathf.Clamp(equippedItem, 1, inventory.Count);

        UpdateInventory();
        UpdatePlayerChildren();
        cameraScript.canEdit = true;
    }
}

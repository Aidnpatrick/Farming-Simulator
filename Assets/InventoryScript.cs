using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;


public class InventoryScript : MonoBehaviour
{
    public GameObject gameCanvas;
    public TMP_Text materialText, coinsText;
    //list of inventory images:
    public GameObject image;
    public List<string> inventory = new List<string>();
    public GameObject player;
    public int equippedItem = 1;
    public int ammo = 10;
    public int materials = 1000;
    public int coins = 1000;
    private int maxInventorySize = 5;
    void Start()
    {
        UpdateInventory();

    }
    void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.digit1Key.wasPressedThisFrame) equippedItem = 1;
        else if (keyboard.digit2Key.wasPressedThisFrame) equippedItem = 2;
        else if (keyboard.digit3Key.wasPressedThisFrame) equippedItem = 3;
        else if (keyboard.digit4Key.wasPressedThisFrame) equippedItem = 4;
        else if (keyboard.digit5Key.wasPressedThisFrame) equippedItem = 5;

        for (int i = 0; i < inventory.Count; i++)
        {
            string itemName = inventory[i];
            Transform itemTransform = player.transform.Find(itemName + (i + 1));
            if (itemTransform == null) continue;

            bool isEquipped = (i == equippedItem - 1);
            itemTransform.gameObject.SetActive(isEquipped);

            if (isEquipped)
            {
                itemTransform.localPosition = new Vector3(0.5f, 0, 0);
                itemTransform.localRotation = Quaternion.identity;
            }
        }
        if (keyboard.qKey.wasPressedThisFrame)
        {
            DropEquippedItem();
        }
        UpdateInventory();
        materialText.text = "Materials: " + materials + "\nCoins: " + coins;
        
    }


    public void UpdateInventory()
    {
        foreach (Transform img in gameCanvas.transform)
            Destroy(img.gameObject);

        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject item = Instantiate(image);
            item.transform.SetParent(gameCanvas.transform, false);

            UnityEngine.UI.Image imgcomp =
                item.GetComponent<UnityEngine.UI.Image>();

            Sprite spriteimg = Resources.Load<Sprite>("Images/" + inventory[i]);

            if (spriteimg == null)
            {
                Debug.LogError("Missing sprite: " + inventory[i]);
                Destroy(item);
                continue;
            }

            imgcomp.sprite = spriteimg;
        }
    }

    public void AddItem(string itemName)
    {
        if (inventory.Count >= maxInventorySize) return;

        GameObject prefab = Resources.Load<GameObject>(itemName);
        if (prefab == null) return;

        GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        clone.name = itemName + (inventory.Count + 1);
        clone.transform.SetParent(player.transform);
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;
        clone.SetActive(false);

        inventory.Add(itemName);
        Debug.Log(itemName);
    }

    public void DropEquippedItem(bool isBlock = false)
    {
        if (inventory.Count == 0) return;

        int dropIndex = equippedItem - 1;
        if (dropIndex < 0 || dropIndex >= inventory.Count) return;

        string itemName = inventory[dropIndex];
        string objectName = itemName + (dropIndex + 1);
        Debug.Log(dropIndex + " " + itemName + " " + objectName);
        
        Transform itemTransform = player.transform.Find(objectName);
        if (itemTransform != null) Destroy(itemTransform.gameObject);

        GameObject lootPrefab = Resources.Load<GameObject>(itemName + "Loot");
        
        if (lootPrefab != null)
        {
            if(!isBlock)
            {
                GameObject dropped = Instantiate(lootPrefab, player.transform.position, Quaternion.identity);

                dropped.tag = "Loot";
                dropped.name = itemName + "Loot";
            }
        }
        else
        {
            Debug.LogError("There isn't a lootprefab for: " + itemName);
        }
        Debug.Log("Dropped " + itemName);
        inventory.RemoveAt(dropIndex);

        if (inventory.Count == 0)
        {
           equippedItem = 1;
           Transform child = transform.GetChild(0);
           Destroy(child);
        }
        else equippedItem = Mathf.Clamp(equippedItem, 1, inventory.Count);

        equippedItem = 1;

    }
}

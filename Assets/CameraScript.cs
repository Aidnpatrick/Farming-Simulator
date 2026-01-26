using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using NUnit.Framework;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Diagnostics.CodeAnalysis;

public class CameraScript : MonoBehaviour
{
    public InventoryScript invScript;
    public PlayerScript playerScript;

    private Vector3 playerCameraPos = new Vector3(0, 0, -5);
    public float interactRange = 3f;

    public GameObject player;
    public GameObject cursorText;

    public bool canEdit = true;
    public bool isOnTile = false;

    public StandScript currentStandScript = null;

    public GameObject fencePrefab, chestPrefab, dirtPrefab, plantPrefab, gravelPrefab;
    
    public bool suppressInteraction = false, menuInteraction = false;

    void Start()
    {
        
        transform.position += new Vector3(25,30,0);

        menuInteraction = true;
    }
    public void StartGame()
    {
        suppressInteraction = false;
        menuInteraction = false;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if(menuInteraction) return;

        if (currentStandScript != null && currentStandScript.isActive)
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);

            if (Mouse.current.leftButton.wasPressedThisFrame)
                return;
        }


        transform.position = player.transform.position + playerCameraPos;
        //if (suppressInteraction)
        /*{
            suppressInteraction = false;
            return;
        }*/

        //if (EventSystem.current != null &&
        /*    EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }*/
        

        Keyboard keyboard = Keyboard.current;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (currentStandScript != null && keyboard.eKey.wasPressedThisFrame)
        {
            currentStandScript.SetActive(false);
            playerScript.canMove = true;
            canEdit = true;
            currentStandScript = null;
            suppressInteraction = true;
            return;
        }

        if (!hit)
        {
            isOnTile = false;
            return;
        }

        isOnTile = true;

        if (hit.collider.gameObject.name.Contains("Tile"))
        {
            TileScript tileScript = hit.collider.gameObject.GetComponent<TileScript>();
            if(hit.collider.transform.childCount == 0){}
                //if(invScript.inventory.Count > 0) CursorText("Add", invScript.inventory[invScript.equippedItem - 1]);
            else{}
                //CursorText("Destroy", hit.collider.transform.GetChild(0).name);

            
            if (Input.GetMouseButtonDown(1))
            {
                string currentItem = invScript.inventory[invScript.equippedItem - 1];

                if (invScript.materials > 0 && !tileScript.isFull)
                {
                    if (currentItem == "FencePlacer")
                    {
                        Build(hit.collider.gameObject, fencePrefab, new Vector3(0,0,1), false);
                        invScript.materials -= 2;
                    }

                    if (currentItem == "ChestPlacer")
                    {
                        Build(hit.collider.gameObject, chestPrefab, new Vector3(0,0,1), false);
                        invScript.materials -=20;                 
                    }
                    if(currentItem == "Shovel")
                    {
                        Build(hit.collider.gameObject, gravelPrefab, new Vector3(0,0,1), false);
                        invScript.materials -= 10;
                    }
                    
                    if (currentItem == "Hoe")
                    {
                        Build(hit.collider.gameObject, dirtPrefab, new Vector3(0,0,1), false);
                        invScript.materials -= 1;
                    }
                }

                if (hit.collider.transform.childCount > 0)
                {
                    Transform child = hit.collider.transform.GetChild(0);
                    DirtScript ds = child.GetComponent<DirtScript>();
                    if (ds == null) return;

                    if (currentItem.Contains("Seed") && child.name.Contains("Dirt") && !ds.isFull)
                    {
                        Build(child.gameObject, plantPrefab, new Vector3(0,0,1), false);
                        ds.isFull = true;
                        ds.NewPlant();
                    }
                }


            }
            if (Input.GetMouseButtonDown(0) && tileScript.isFull)
            {
                Transform child = hit.collider.transform.GetChild(0);

                // ----- DIRT -----
                if (child.name.Contains("Dirt") && child.childCount > 0)
                {
                    Destroy(child.GetChild(0).gameObject);

                    DirtScript ds = child.GetComponent<DirtScript>();
                    ds.isFull = false;

                    invScript.materials++;
                    if (ds.isGrown)
                        invScript.AddStock("Wheat", 1, 2);


                    return;
                }

                // ----- TREE -----
                if (child.name.Contains("Tree") && child.childCount > 1)
                {
                    //CursorText("Shake",child.name);
                    Transform apple = child.GetChild(1);

                    if (apple.name.Contains("Apple"))
                    {
                        TreeScript treeScript = child.GetComponent<TreeScript>();
                        if (treeScript.apples.Count == 0) return;
                        invScript.AddStock("Apple", 1, 1);
                        treeScript.apples.RemoveAt(treeScript.apples.Count - 1);
                        Destroy(apple.gameObject);
                        return;
                    }
                }
                // ----- DEFAULT REMOVE -----
                Destroy(child.gameObject);
                tileScript.isFull = false;
                invScript.materials++;
            }

            if (keyboard.eKey.wasPressedThisFrame)
            {
                Debug.Log("Pressed on tile");
            }
        }
        if(Input.GetMouseButtonDown(0))
        {
            if(hit.collider.name.Contains("Fence") || hit.collider.name.Contains("Weed") || hit.collider.name.Contains("Tree"))
            {
                //CursorText("Break",hit.collider.name);

                Transform parent = hit.collider.transform.parent;
                TileScript ts = parent.GetComponent<TileScript>();
                ts.isFull = false;
                if(hit.collider.name.Contains("Fence"))
                    invScript.materials++;
                Destroy(hit.collider.gameObject);
            }

            if(hit.collider.name.Contains("Apple"))
            {
                //CursorText("Get",hit.collider.name);

                TreeScript treeScript = hit.collider.transform.parent.GetComponent<TreeScript>();
                treeScript.apples.RemoveAt(treeScript.apples.Count - 1);
                invScript.AddStock("Apple", 1, 1);

                Destroy(hit.collider.gameObject);
            }
        }
        if (hit.collider.name.Contains("Stand"))
        {
            //CursorText("Open",hit.collider.name);

            float distance = Vector3.Distance(player.transform.position, hit.collider.transform.position);
            StandScript standScript = hit.collider.GetComponentInParent<StandScript>();

            if (standScript == null) return;
            

            if (keyboard.eKey.wasPressedThisFrame &&
                distance <= 15 &&
                currentStandScript == null)
            {
                standScript.SetActive(true);
                playerScript.canMove = false;
                canEdit = false;
                currentStandScript = standScript;
                suppressInteraction = true;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }


        Transform hitTransform = hit.collider.transform;

        if (!hit.collider.name.Contains("Tile"))
        {
            CursorText("", hit.collider.name);
            return;
        }

        if (hitTransform.childCount == 0)
        {
            CursorText("", "");
            return;
        }

        Transform firstChild = hitTransform.GetChild(0);

        if (firstChild.childCount == 0)
        {
            CursorText("", firstChild.name);
            return;
        }

        if (firstChild.name.Contains("Tree"))
        {
            if (firstChild.childCount > 1)
                CursorText("", firstChild.GetChild(1).name);
            else
                CursorText("", firstChild.name);

            return;
        }

        CursorText("", firstChild.GetChild(0).name);

            }

    public GameObject Build(GameObject parent, GameObject prefab, Vector3 position, bool isNatural, string[] sprites = null)
    {
        if (parent.name.Contains("Tile"))
            parent.GetComponent<TileScript>().isFull = true;

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        obj.transform.SetParent(parent.transform, false);
        obj.transform.localPosition = position;
        obj.name = prefab.name + "Clone";
        if(sprites != null)
        {
            SpriteRenderer objSprite = obj.GetComponent<SpriteRenderer>();
            Sprite spriteimg = Resources.Load<Sprite>("Images/" + sprites[Random.Range(0, sprites.Length)]);
            objSprite.sprite = spriteimg;
        }
        
        if(!isNatural)
            invScript.materials--;


        return obj;
    }

    public void CursorText(string verb, string collisionName)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        cursorText.transform.position = mouseWorldPos;
        Vector3 pos = cursorText.transform.position;
        pos.z = 1f;
        collisionName = collisionName.Replace("Clone", "");
        cursorText.transform.position = pos;
        cursorText.GetComponent<TMP_Text>().text = verb + "" + collisionName;
    }
}

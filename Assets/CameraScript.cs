using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    public InventoryScript invScript;
    public PlayerScript playerScript;

    private Vector3 playerCameraPos = new Vector3(0, 0, -5);
    public float interactRange = 3f;

    public GameObject player;

    public bool canEdit = true;
    public bool isOnTile = false;

    public StandScript currentStandScript = null;

    public GameObject fencePrefab, chestPrefab, dirtPrefab, plantPrefab;

    private bool suppressInteraction = false;

    void Update()
    {
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

            if (Input.GetMouseButtonDown(1))
            {
                string currentItem = invScript.inventory[invScript.equippedItem - 1];

                if (invScript.materials > 0 && !tileScript.isFull)
                {
                    if (currentItem == "FencePlacer")
                        Build(hit.collider.gameObject, fencePrefab, hit.collider.transform.position);

                    if (currentItem == "ChestPlacer")
                        Build(hit.collider.gameObject, chestPrefab, hit.collider.transform.position);

                    if (currentItem == "Hoe")
                        Build(hit.collider.gameObject, dirtPrefab, hit.collider.transform.position);
                }

                if (hit.collider.transform.childCount > 0)
                {
                    Transform child = hit.collider.transform.GetChild(0);
                    DirtScript ds = child.GetComponent<DirtScript>();
                    if (ds == null) return;

                    if (currentItem.Contains("Seed") && child.name.Contains("Dirt") && !ds.isFull)
                    {
                        Build(child.gameObject, plantPrefab, hit.collider.transform.position);
                        ds.isFull = true;
                        ds.NewPlant();
                    }
                }
            }

            if (Input.GetMouseButtonDown(0) && tileScript.isFull)
            {
                Transform child = hit.collider.transform.GetChild(0);

                if (child.name.Contains("Dirt") && child.childCount > 0)
                {
                    Destroy(child.GetChild(0).gameObject);
                    child.GetComponent<DirtScript>().isFull = false;
                    invScript.materials++;
                    return;
                }

                Destroy(child.gameObject);
                tileScript.isFull = false;
                invScript.materials++;
            }
        }

        if (hit.collider.name.Contains("Stand"))
        {
            float distance = Vector3.Distance(player.transform.position, hit.collider.transform.position);
            StandScript standScript = hit.collider.GetComponentInParent<StandScript>();

            if (standScript == null) return;

            if (keyboard.eKey.wasPressedThisFrame &&
                distance <= 8 &&
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
    }

    void Build(GameObject parent, GameObject prefab, Vector3 position)
    {
        if (parent.name.Contains("Tile"))
            parent.GetComponent<TileScript>().isFull = true;

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        obj.transform.SetParent(parent.transform);
        obj.name = prefab.name + "Clone";

        invScript.materials--;
    }
}

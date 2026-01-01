using UnityEngine;
using UnityEngine.InputSystem;
public class BlockerPlacerScript : MonoBehaviour
{
    private CameraScript camScript;
    private InventoryScript invScript;
    void Start()
    {
        camScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        invScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1) && camScript.isOnTile == true)
        {
            invScript.DropEquippedItem(true);
        }
        
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public InventoryScript inventoryScript;
    private Vector2 moveInput;
    public bool canMove = true;
    public bool canGrab = false;
    private float moveSpeed = 5;
    private string currentLoot = "";
    public float health = 10;
    public Vector3 startinglocation = new Vector3(30,30,1);
    private GameObject currentLootObject = null;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = startinglocation;
    }

    void Update()
    {
        moveInput = Vector2.zero;
        if (canMove)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;            
        }
        moveInput = moveInput.normalized;
        Keyboard keyboard = Keyboard.current;
        if(canGrab && keyboard.eKey.wasPressedThisFrame && canMove)
        {
            inventoryScript.AddItem(currentLoot);
            Destroy(currentLootObject);
            canGrab = false;
        }

    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Loot"))
        {
            currentLoot = other.gameObject.name.Replace("Loot", "");
            currentLootObject = other.gameObject;
            canGrab = true;
        }
    }
    void OnTriggerEnter(Collider other)
    {
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Loot"))
        {
            canGrab = false;
        }
        
    }


    [System.Obsolete]
    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }
}

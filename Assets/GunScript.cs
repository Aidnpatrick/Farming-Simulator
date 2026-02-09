using UnityEngine;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    public GameObject bulletsPrefab;
    private InventoryScript invScript;
    private float bulletSpeed = 20f;
    public float r = 1f;
    public float coolDown;
    public AudioSource audioSource;
    public AudioClip gunShot;
    void Start()
    {
        invScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
    }
    void Update()
    {
        r-= 0.75f;
        if(r < 0) r = 0;
        coolDown = Mathf.Clamp(coolDown - Time.deltaTime, 0, 10);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        Vector3 direction = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        if (Input.GetMouseButtonDown(0) && invScript.ammo > 0 && coolDown <= 0)
        {
            audioSource.PlayOneShot(gunShot);
            invScript.ammo--;

            Transform barrel = transform.GetChild(0);
            GameObject bp = Instantiate(bulletsPrefab, barrel.position, transform.rotation);
            bp.transform.Rotate(0, 0, Random.Range(-r,r));
            bp.name = "Bullet";
            Rigidbody2D rb = bp.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = bp.transform.right * bulletSpeed;
            }
            r = Mathf.Clamp(r + 25, 0, 55);
            Destroy(bp, 2f);
            coolDown = 0.25f;
        }
    }
}

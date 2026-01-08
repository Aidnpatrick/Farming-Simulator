using UnityEngine;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    public int ammo = 10;
    public GameObject bulletsPrefab;
    private float bulletSpeed = 20f;
    private float r = 1f;
    void Update()
    {
        r-= 0.35f;
        if(r < 0) r = 0;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        Vector3 direction = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        if (Input.GetMouseButtonDown(0) && ammo > 0)
        {
            ammo--;

            Transform barrel = transform.GetChild(0);
            GameObject bp = Instantiate(bulletsPrefab, barrel.position, transform.rotation);
            bp.transform.Rotate(0, 0, Random.Range(-r,r));
            bp.name = "Bullet";
            Rigidbody2D rb = bp.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = bp.transform.right * bulletSpeed;
            }

            r += 10;
            Destroy(bp, 2f);
        }
    }
}

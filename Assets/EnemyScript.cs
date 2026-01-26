using System.Security.Cryptography;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private GameControlScript gameControlScript;
    public float speed = 10f;
    public float health = 100f;
    public float visionRange = 5f;

    public Vector3 lastKnownLocation;
    public bool canSeePlayer;

    public float movingCooldown;
    public float attackingCooldown;
    public GameObject targetCrop = null;
    private GameObject player;

    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        player = GameObject.Find("Player");
        movingCooldown = 0f;
        attackingCooldown = 0f;
    }

    void Update()
    {
        if (player == null) return;

        movingCooldown -= Time.deltaTime;
        attackingCooldown -= Time.deltaTime;

        float distanceFromPlayer =
            Vector3.Distance(player.transform.position, transform.position);

        canSeePlayer = distanceFromPlayer <= visionRange;

        if (canSeePlayer)
        {
            lastKnownLocation = player.transform.position;
            movingCooldown = 15f;
            MoveTowards(player.transform.position);
        }
        else if (movingCooldown <= 0f)
        {
            lastKnownLocation = FindRandomPos();
            movingCooldown = 10f;
        }
        else
        {
            FindTargetCrop();
            if(targetCrop == null) 
                MoveTowards(lastKnownLocation);
            else 
                MoveTowards(targetCrop.transform.position);
        }


        if(gameControlScript.isDay)
        {
            health -= 0.5f;
        }
        
    }
    void FindTargetCrop()
    {
        foreach (GameObject tile in gameControlScript.tiles)
        {
            DirtScript ds = tile.GetComponentInChildren<DirtScript>();

            if (ds == null) continue;
            if (!ds.isFull) continue;

            if (Vector3.Distance(tile.transform.position, transform.position) < 8f)
            {
                targetCrop = tile;
                return;
            }
        }

        targetCrop = null;
    }


    void MoveTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.z = 0f;

        if (direction.magnitude < 0.1f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    Vector3 FindRandomPos()
    {
        
        Vector2 offset = new Vector2(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)
        );
        if(offset.x < 0)           
            offset = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        return transform.position + (Vector3)offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (attackingCooldown <= 0f)
            {
                PlayerScript ps = collision.GetComponent<PlayerScript>();
                if (ps != null)
                {
                    ps.health -= 10f;
                    attackingCooldown = 3f;
                }
            }
        }

        if (collision.CompareTag("Bullet"))
        {
            health -= 20f;
            Destroy(collision.gameObject);

            if (health <= 0f)
            {
                Destroy(gameObject);
            }
        }
        if (collision.CompareTag("Tile"))
        {
            GameObject tile = collision.gameObject;

            if (tile == targetCrop)
            {
                DirtScript ds = tile.GetComponentInChildren<DirtScript>();
                if (ds == null) return;

                if (ds.transform.childCount > 0)
                {
                    Destroy(ds.transform.GetChild(0).gameObject);
                }
                ds.isFull = false;
                targetCrop = null;
            }
        }

    }
}

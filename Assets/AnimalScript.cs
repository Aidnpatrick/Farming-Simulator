using TMPro;
using UnityEngine;

public class AnimalScript : NPC
{
    private InventoryScript invScript;
    private GameObject player;
    public float eatingCooldown = 0f;
    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        invScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
        player = GameObject.Find("Player");
        movingCooldown = 0f; 
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        movingCooldown -= Time.deltaTime;
        attackingCooldown -= Time.deltaTime;
        eatingCooldown -= Time.deltaTime;

        float distanceFromPlayer =
            Vector3.Distance(player.transform.position, transform.position);

        canSeePlayer = distanceFromPlayer <= visionRange;

        if (canSeePlayer && invScript.inventory.Count > 0 && invScript.inventory[invScript.equippedItem - 1] == "Seeds")
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
            targetCrop = FindTargetWeed(gameControlScript.tiles);
            if(targetCrop == null) 
                MoveTowards(lastKnownLocation);
            else if(eatingCooldown <= 0)
                MoveTowards(targetCrop.transform.position);
        }
        rb = GetComponent<Rigidbody2D>();

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
        {
            GameObject tile = collision.gameObject;

            if (tile == targetCrop)
            {
                if(tile.transform.childCount < 1) return;
                eatingCooldown = 10;
                TileScript ts = tile.GetComponent<TileScript>();
                Destroy(tile.transform.GetChild(0).gameObject);
                ts.isFull = false;
                targetCrop = null;
                movingCooldown = 0;
            }
        }
        if (collision.CompareTag("Bullet"))
        {
            health -= 20f;
            gameControlScript.Blood(1, gameObject);
            Destroy(collision.gameObject);

            if (health <= 0f)
            {
                Destroy(gameObject);
            }
        }
        if(collision.CompareTag("Enemy"))
        {
            EnemyScript es = collision.GetComponent<EnemyScript>();
            if(es.attackingCooldown <= 0)
            {
                gameControlScript.Blood(1, gameObject);
                health -= 20;
                es.attackingCooldown = 2;
                if(health <= 0) Destroy(gameObject);
            }
        }
    }
}

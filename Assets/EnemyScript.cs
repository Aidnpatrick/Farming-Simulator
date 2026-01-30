using UnityEngine;

public class EnemyScript : NPC
{
    private GameObject player;
    public GameObject targetAnimal = null;

    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        player = GameObject.Find("Player");
        movingCooldown = 0f;
        attackingCooldown = 0f;
    }

    void Update()
    {
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
            targetCrop = FindTargetCrop(gameControlScript.tiles);
            targetAnimal = FindTargetAnimal(gameControlScript.animals);
            if(movingCooldown <= 0)
                MoveTowards(lastKnownLocation);
            else if(targetCrop != null)
                MoveTowards(targetCrop.transform.position);
            else if(targetAnimal != null)
                MoveTowards(targetAnimal.transform.position);
            /*
            if(targetCrop == null) 
                MoveTowards(lastKnownLocation);
            else if(targetCrop == null)
                MoveTowards(targetCrop.transform.position);
            else if(targetAnimal != null)
                MoveTowards(targetAnimal.transform.position);
            */
        }


        if(gameControlScript.isDay)
        {
            health -= 0.5f;
        }
        
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
            gameControlScript.Blood(1, gameObject);
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

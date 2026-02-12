using UnityEngine;

public class EnemyScript : NPC
{
    private GameObject player;
    public GameObject targetAnimal = null;
    public float animalSoundCooldown = 0f;
    public AudioSource audioSource;
    public AudioClip zombieNoise;
    void Start()
    {
        speed = 1.5f;
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        player = GameObject.Find("Player");
        movingCooldown = 0f;
        attackingCooldown = 0f;
        rb = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate()
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
        }


        if(gameControlScript.isDay) health -= 1.5f;
        if(health <= 0) {
            gameControlScript.Blood(1, gameObject);
            Destroy(gameObject);
        }
        rb = GetComponent<Rigidbody2D>();
        if(animalSoundCooldown <= 0)
        {
            audioSource.PlayOneShot(zombieNoise);
            animalSoundCooldown = Random.Range(0,25);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (attackingCooldown <= 0f)
            {
                PlayerScript ps = collision.GetComponent<PlayerScript>();
                if (ps != null)
                {
                    ps.health -= 1f;
                    attackingCooldown = 0.5f;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

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

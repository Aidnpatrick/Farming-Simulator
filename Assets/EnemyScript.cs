using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //walking to player | patrolling
    public float[] weights = {1f, 1f};
    public float[] outputs = {0f, 0f};
    public Vector3 lastKnownLocation;
    public bool canSeePlayer;
    public float movingCooldown = 0f;
    public float speed = 10;
    public float health = 100;
    private GameObject player;


    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        movingCooldown -= Time.deltaTime;
        float distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);
        canSeePlayer = distanceFromPlayer <= 5f;

        if (canSeePlayer)
        {
            lastKnownLocation = player.transform.position;
            movingCooldown = 15f;
            Actions(0);
        }
        else if (movingCooldown <= 0f)
        {
            lastKnownLocation = FindRandomPos();
            movingCooldown = 10f;
            Actions(1);
        }
    }
    void Actions(int action)
    {
        Vector3 direction = Vector3.zero;
        switch(action)
        {
            case 0: 
                direction = player.transform.position - transform.position;
                break;

            case 1: 
                direction = lastKnownLocation - transform.position;
                break;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    public Vector3 FindRandomPos()
    {
        Vector3 pos = new Vector2(
            Random.Range(-10, 10),
            Random.Range(-10, 10)
        );
        return transform.position + pos;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Player")
        {
            PlayerScript ps = collision.GetComponent<PlayerScript>();
            ps.health -= 10;
            
        }
        if(collision.name.Contains("Bullet"))
        {
            health -=20;
            Destroy(collision.gameObject);
            if(health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

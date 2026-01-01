using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class EnemyScript : MonoBehaviour
{
    //walking to player | patrolling
    public float[] weights = {1f, 1f};
    public float[] outputs = {0f, 0f};
    public Vector3 lastKnownLocation;
    public bool canSeePlayer;
    public float movingCooldown = 0f;
    public float activationLevel = 0f;
    public float speed = 10;
    private GameObject player;


    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        movingCooldown -= Time.deltaTime;
        float distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);
        canSeePlayer = distanceFromPlayer <= 8f;

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
        switch(action)
        {
            case 0: //walking to player
                transform.LookAt(player.transform.position);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                break;
            
            case 1: //patroling
                transform.LookAt(lastKnownLocation);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                break;
        }
    }
    Vector3 FindRandomPos()
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
    }
}

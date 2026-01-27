using System;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool isFull = false;
    public bool isFertile;

    void Update()
    {
        if(transform.childCount == 0) isFull = false;
        else isFull = true;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Bullet" && isFull == true)
        {
            Destroy(collision.gameObject);
        }
    }
}

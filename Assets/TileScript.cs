using System;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool isFull = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Bullet" && isFull == true)
        {
            Destroy(collision.gameObject);
        }
    }
}

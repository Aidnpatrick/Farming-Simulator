using System;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool isFull = false;
    public bool isFertile, isWater;
    public int tileID;
    void Update()
    {
        if(transform.childCount == 0) isFull = false;
        else isFull = true;
        /*
        if(transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            if(child.name.Contains("Water"))
            {
                if(child.childCount > 0)
                {
                    toggleCollider(false, child.transform);
                }
                else
                {
                    toggleCollider(true, child.transform);
                }
            }
        }
        */
    }

    public void toggleCollider(bool toggle, Transform child)
    {
        Collider2D[] colliders = child.GetComponents<Collider2D>();

        foreach (Collider2D c in colliders)
        {
            c.enabled = toggle;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Bullet" && isFull == true)
        {
            Transform child = transform.GetChild(0);
            string childName = child.name;
            if(childName.Contains("Fence") || childName.Contains("Chest") || childName.Contains("Tree"))
                Destroy(collision.gameObject);
        }
    }
}

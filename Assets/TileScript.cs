using System;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool isFull = false;
    public string childName = "";
    public string grandChildName = "";
    /*void Update()
    {
        Transform child = transform.GetChild(0);
        childName = child.name != null ? child.name : "";

        grandChildName = child.GetChild(0) != null ? child.GetChild(0).name : "";
    }*/
}

using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    private InventoryScript invScript;
    private CameraScript cameraScript;
    public GameObject applePrefab;
    public bool isFull = false;
    public List<GameObject> apples = new List<GameObject>();
    void Start()
    {
        invScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        for(int i = 0; i < 2; i++)
        {
            if(Random.Range(0f,1f) > 0.5f) continue;
            Vector3 randomOffSet = new Vector3(Random.Range(-0.1f,0.1f), Random.Range(-0.1f,0.1f), 0);
            apples.Add(cameraScript.Build(gameObject, applePrefab, randomOffSet, true));
        }
    }
    void Update()
    {
        if(Random.Range(0f,100f) < 0.2f && gameObject.transform.childCount < 4)
        {
            Vector3 randomOffSet = new Vector3(Random.Range(-0.1f,0.1f), Random.Range(-0.1f,0.1f), 0);
            apples.Add(cameraScript.Build(gameObject, applePrefab, randomOffSet, true));
        }
    }
}

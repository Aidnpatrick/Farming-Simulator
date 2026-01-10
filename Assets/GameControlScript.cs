using UnityEngine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

public class GameControlScript : MonoBehaviour
{
    public float time = 0f;
    public bool isDay = true;
    public CameraScript cameraScript;
    public GameObject enemyPrefab, weedPrefab, treePrefab, zombiePrefab;
    public int weedCounter = 0;
    private GameObject[] tiles;
    private string[] treeTypes = {"Tree4"};
    private GameObject[] trees;
    void Start()
    {
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        StartCoroutine(DayNightCycle());
        StartCoroutine(Weeds());
        //GameObject clone = Instantiate(enemyPrefab, new Vector3(0,0,1), Quaternion.identity);
        SpawnWorld();

    }

    void Update()
    {
        trees = GameObject.FindGameObjectsWithTag("Tree");
        time += Time.deltaTime;
        
    }
    IEnumerator DayNightCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            Instantiate(zombiePrefab, new Vector3(0,0,1), Quaternion.identity);

            Debug.Log("Is day:" + isDay);
            isDay = !isDay;
        }
    }


    IEnumerator Weeds()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            GameObject tileIndex;
            TileScript ts;
            for(int i = 0; i < 15; i++)
            {
                if(weedCounter >= 100) continue;
                tileIndex = tiles[Random.Range(0,tiles.Length)];
                ts = tileIndex.GetComponent<TileScript>();
                if(!ts.isFull)
                    cameraScript.Build(tileIndex, weedPrefab, new Vector3(0,0,1), true);
                weedCounter++;
            }

            for(int i = 0; i < 4; i++)
            {

                tileIndex = tiles[Random.Range(0,tiles.Length)];
                ts = tileIndex.GetComponent<TileScript>();
                GameObject treeClone;
                if(!ts.isFull)
                {
                    treeClone = cameraScript.Build(tileIndex, treePrefab, new Vector3(0,1f,1), true, treeTypes);
                    foreach(GameObject k in trees)
                    {
                        if(Vector3.Distance(treeClone.transform.position, k.transform.position) <= 8)
                        {
                            Destroy(treeClone);
                            break;
                        }
                    }
                }

            }
            
        }
    }

    void SpawnWorld()
    {
            GameObject tileIndex;
            TileScript ts;
            for(int i = 0; i < 70; i++)
            {
                if(weedCounter >= 20) continue;
                tileIndex = tiles[Random.Range(0,tiles.Length)];
                ts = tileIndex.GetComponent<TileScript>();
                if(!ts.isFull)
                    cameraScript.Build(tileIndex, weedPrefab, new Vector3(0,0,1), true);
            }

            for(int i = 0; i < 30; i++)
            {

                tileIndex = tiles[Random.Range(0,tiles.Length)];
                ts = tileIndex.GetComponent<TileScript>();
                GameObject treeClone;
                if(!ts.isFull)
                {
                    treeClone = cameraScript.Build(tileIndex, treePrefab, new Vector3(0,1f,1), true, treeTypes);
                    //shadowClone = cameraScript.Build(treeClone, shadowPrefab, new Vector3(0,0,1), true);
                    foreach(GameObject k in trees)
                    {
                        if(Vector3.Distance(treeClone.transform.position, k.transform.position) <= 8)
                        {
                            Destroy(treeClone);
                            break;
                        }
                    }
                }

            }
    }

    IEnumerator SpawnZombies()
    {
        
        yield return new WaitForSeconds(1f);
    }
}


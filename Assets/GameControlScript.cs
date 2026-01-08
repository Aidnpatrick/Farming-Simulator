using UnityEngine;
using System.Collections;
using TMPro;

public class GameControlScript : MonoBehaviour
{
    public float time = 0f;
    public bool isDay = true;
    public CameraScript cameraScript;
    public GameObject enemyPrefab, weedPrefab, treePrefab;
    private GameObject[] tiles;
    private string[] treeTypes = {"Tree1","Tree2","Tree3"};
    void Start()
    {
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        StartCoroutine(DayNightCycle());
        StartCoroutine(Weeds());
        //GameObject clone = Instantiate(enemyPrefab, new Vector3(0,0,1), Quaternion.identity);
        
    }

    void Update()
    {
        time += Time.deltaTime;
        
    }
    IEnumerator DayNightCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            Debug.Log("Is day:" + isDay);
            isDay = !isDay;
        }
    }

    IEnumerator Weeds()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            for(int i = 0; i < 2; i++)
            {
                GameObject tileIndex = tiles[Random.Range(0,tiles.Length)];
                TileScript ts = tileIndex.GetComponent<TileScript>();
                if(!ts.isFull)
                    cameraScript.Build(tileIndex, weedPrefab, new Vector3(0,0,1), true);
                tileIndex = tiles[Random.Range(0,tiles.Length)];
                ts = tileIndex.GetComponent<TileScript>();
                if(!ts.isFull)
                    cameraScript.Build(tileIndex, treePrefab, new Vector3(0,1f,1), true, treeTypes);

            }
            
        }
    }


    IEnumerator SpawnZombies()
    {
        
        yield return new WaitForSeconds(1f);
    }
}


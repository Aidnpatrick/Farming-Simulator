using UnityEngine;
using System.Collections;

using UnityEngine.InputSystem;
using System.Security.Cryptography;

[System.Serializable]
public class WorldObjectData
{
    public string id;
    public int tileIndex;
}


public class GameControlScript : MonoBehaviour
{
    //color from backgroud grass: 57B200
    
    public CameraScript cameraScript;
    public InventoryScript inventoryScript;
    public GameObject gameCanvas;
    public GameObject gameInstructions, tutorial;
    public StandScript standScript;
    public GameObject menuCanva;
    public GameObject buyStand, sellStand;
    public GameObject darkness;
    public GameObject weedPrefab;
    public GameObject treePrefab;
    public GameObject zombiePrefab;
    public GameObject animalPrefab;
    public GameObject bloodPrefab;
    public GameObject wavePrefab;
    public GameObject[] tiles;
    public GameObject[] animals;
    public GameObject[] enemies;
    private string[] treeTypes = { "Tree4" };
    private string[] bloodSprites = {"Blood1", "Blood2", "Blood3"};
    public bool isDay = true;
    public bool gameInstructionsOpen = false, tutorialOpen = false;
    public AudioSource audioSource;
    public AudioClip pop;
    

    public void Start()
    {
        gameCanvas.SetActive(false);
        gameInstructions.SetActive(false);
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        for(int i = 0; i < 5; i++)
            SpawnWorld();

    }
    public void ToggleRules()
    {
        gameInstructionsOpen = !gameInstructionsOpen;
        gameInstructions.SetActive(gameInstructionsOpen);
        gameInstructions.transform.SetAsLastSibling();
        audioSource.PlayOneShot(pop);

    }
    public void ToggleTutorial()
    {
        tutorialOpen = !tutorialOpen;
        tutorial.SetActive(tutorialOpen);
        tutorial.transform.SetAsLastSibling();
    }
    public void StartGame()
    {
        audioSource.PlayOneShot(pop);

        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        standScript = GameObject.Find("Stand").GetComponent<StandScript>();
        inventoryScript = FindObjectOfType<InventoryScript>();
        menuCanva.SetActive(false);
        gameCanvas.SetActive(true);

        tiles = GameObject.FindGameObjectsWithTag("Tile");
        System.Array.Sort(tiles, (a, b) => a.name.CompareTo(b.name));

        StartCoroutine(DayNightCycle());
        StartCoroutine(Weeds());
        /*
        SaveData data = SaveSystem.Load();
        if (data != null)
            LoadWorld(data);
        else
            SpawnWorld();
        */
        SpawnWorld();
        StartCoroutine(StartWaves());
    }

    void Update()
    {
        /*
        if (Keyboard.current.oKey.wasPressedThisFrame)
            SaveSystem.Save(BuildSaveData());

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            SaveData data = SaveSystem.Load();
            if (data != null)
                LoadWorld(data);
        }
        */

        if(Keyboard.current.oKey.wasPressedThisFrame)
        {
            Instantiate(zombiePrefab, new Vector3(1,1,1), Quaternion.identity);
        }
        if(Keyboard.current.pKey.wasPressedThisFrame)
        {
            Instantiate(animalPrefab, new Vector3(1,1,1), Quaternion.identity);
        }

        animals = GameObject.FindGameObjectsWithTag("Animal");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        darkness.SetActive(!isDay);

        foreach(GameObject t in GameObject.FindGameObjectsWithTag("Torch"))
            t.transform.GetChild(0).gameObject.SetActive(!isDay);
    }

    void SpawnWorld()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject tile = tiles[Random.Range(0, tiles.Length)];
            if (!tile.GetComponent<TileScript>().isFull && tile.GetComponent<TileScript>().isFertile && GameObject.FindGameObjectsWithTag("Weed").Length <= 80)
                cameraScript.Build(tile, weedPrefab, new Vector3(0, 0, 1), true);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject tile = tiles[Random.Range(0, tiles.Length)];
            
            GameObject treeClone;
            if (!tile.GetComponent<TileScript>().isFull)
            {

                treeClone = cameraScript.Build(tile, treePrefab, new Vector3(0, 1f, 1), true, treeTypes);
                Vector3 tilePos = treeClone.transform.position; 

                foreach(GameObject otherTile in tiles)
                {
                    if (otherTile == tile) 
                        continue;

                    TreeScript tree = otherTile.GetComponentInChildren<TreeScript>();
                    if (tree == null) 
                        continue;

                    if(Vector3.Distance(tilePos, tree.transform.position) < 5)
                    {
                        Destroy(treeClone);
                        break;
                    }
                }
                if(!tile.GetComponent<TileScript>().isFertile)
                {
                    treeClone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/PalmTree");
                }
                
                if(Vector3.Distance(buyStand.transform.position, treeClone.transform.position) < 10 && Vector3.Distance(sellStand.transform.position, treeClone.transform.position) < 10)
                {
                    Destroy(treeClone);
                }
            }
        }
    }

    IEnumerator Weeds()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            GameObject tile = tiles[Random.Range(0, tiles.Length)];
            if (!tile.GetComponent<TileScript>().isFull)
                cameraScript.Build(tile, weedPrefab, new Vector3(0, 0, 1), true);
            SpawnWorld();
        }
        
    }
    IEnumerator StartWaves()
    {
        while (true)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-10f, 50f),
                Random.Range(10f, 50f),
                1f
            );

            GameObject waveClone = Instantiate(wavePrefab, randomPosition, Quaternion.identity);

            float speed = Random.Range(1f, 3f);

            StartCoroutine(MoveWave(waveClone, speed));

            Destroy(waveClone, 2f);
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator MoveWave(GameObject wave, float speed)
    {
        float elapsed = 0f;
        while (wave != null && elapsed < 2f) 
        {
            wave.transform.Translate(Vector3.left * speed * Time.deltaTime);
            wave.transform.Translate(Vector3.up * (speed / 2f) * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null;
        }
}
    IEnumerator ZombiesAndSheep()
    {
        for(int i = 0; i < 15; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(2,30),
                Random.Range(2,30),
                1
            );
            if(isDay && animals.Length < 6) Instantiate(animalPrefab, randomPosition, Quaternion.identity);
            else if(!isDay && enemies.Length < 4) 
            {
                GameObject zombieClone = Instantiate(zombiePrefab, randomPosition, Quaternion.identity);
                foreach(GameObject t in GameObject.FindGameObjectsWithTag("Torch"))
                {
                    if(Vector3.Distance(zombieClone.transform.position, t.transform.position) < 10)
                    {
                        Destroy(zombieClone);
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(5f);
        }
    }

    public void Blood(int amount, GameObject target)
    {
        for(int i = 0; i < amount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-0.5f,0.5f),
                Random.Range(-0.5f,0.5f),
                1
            );
            GameObject bloodClone = Instantiate(bloodPrefab, target.transform.position + randomPosition, Quaternion.identity);
            bloodClone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + bloodSprites[Random.Range(0, bloodSprites.Length)]);
            Destroy(bloodClone, 20);
        }
    }
    IEnumerator DayNightCycle()
    {
        while (true)
        {
            StartCoroutine(ZombiesAndSheep());
            yield return new WaitForSeconds(60f);
            isDay = !isDay;
            SpawnWorld();

        }
    }

    
}

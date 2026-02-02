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
/*
[System.Serializable]
public class SaveData
{
    public List<WorldObjectData> worldObjects = new List<WorldObjectData>();
    public List<string> inventorySaved;
    public List<Stock> stockSaved;
}
*/
/*
public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    public static SaveData Load()
    {
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
    }
}

*/

public class GameControlScript : MonoBehaviour
{
    //color from backgroud grass: 57B200
    
    public CameraScript cameraScript;
    public InventoryScript inventoryScript;
    public GameObject gameCanvas;
    public GameObject gameInstructions;
    public StandScript standScript;
    public GameObject menuCanva;
    public GameObject buyStand, sellStand;
    public GameObject darkness;
    public GameObject weedPrefab;
    public GameObject treePrefab;
    public GameObject zombiePrefab;
    public GameObject animalPrefab;
    public GameObject bloodPrefab;
    public GameObject[] tiles;
    public GameObject[] animals;
    public GameObject[] enemies;
    private string[] treeTypes = { "Tree4" };
    private string[] bloodSprites = {"Blood1", "Blood2", "Blood3"};
    public bool isDay = true;
    public bool gameInstructionsOpen = false;

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
    }
    public void StartGame()
    {
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
            Instantiate(zombiePrefab, new Vector3(0,0,1), Quaternion.identity);
        }
        if(Keyboard.current.pKey.wasPressedThisFrame)
        {
            Instantiate(animalPrefab, new Vector3(0,0,1), Quaternion.identity);
        }

        animals = GameObject.FindGameObjectsWithTag("Animal");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        darkness.SetActive(!isDay);

        foreach(GameObject t in GameObject.FindGameObjectsWithTag("Torch"))
            t.transform.GetChild(0).gameObject.SetActive(!isDay);
    }

/*
    SaveData BuildSaveData()
    {
        SaveData data = new SaveData();

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].transform.childCount > 0)
            {
                string name = tiles[i].transform.GetChild(0).name.Replace("(Clone)", "");
                if (name.Contains("Weed")) name = "Weed";
                if (name.Contains("Tree")) name = "Tree";

                data.worldObjects.Add(new WorldObjectData
                {
                    id = name,
                    tileIndex = i
                });
            }
        }

        data.inventorySaved = inventoryScript.inventory;
        data.stockSaved = inventoryScript.stocks;
        return data;
    }


    void LoadWorld(SaveData data)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Weed"))
            Destroy(obj);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Tree"))
            Destroy(obj);

        foreach (WorldObjectData obj in data.worldObjects)
        {
            if (obj.tileIndex < 0 || obj.tileIndex >= tiles.Length) continue;

            if (obj.id == "Weed")
                cameraScript.Build(tiles[obj.tileIndex], weedPrefab, new Vector3(0, 0, 1), true);

            if (obj.id == "Tree")
                cameraScript.Build(tiles[obj.tileIndex], treePrefab, new Vector3(0, 1f, 1), true, treeTypes);
        }

        inventoryScript.inventory = data.inventorySaved;
        inventoryScript.stocks = data.stockSaved;
    }
*/
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
    IEnumerator ZombiesAndSheep()
    {
        for(int i = 0; i < 15; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(2,30),
                Random.Range(2,30),
                1
            );
            if(isDay && animals.Length < 10) Instantiate(animalPrefab, randomPosition, Quaternion.identity);
            else if(!isDay && enemies.Length < 5) 
            {
                GameObject zombieClone = Instantiate(zombiePrefab, randomPosition, Quaternion.identity);
                foreach(GameObject t in GameObject.FindGameObjectsWithTag("Torches"))
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
            yield return new WaitForSeconds(60f);
            isDay = !isDay;
            StartCoroutine(ZombiesAndSheep());
        }
    }
}

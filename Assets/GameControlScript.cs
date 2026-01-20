using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.InputSystem;

#region SAVE DATA

[System.Serializable]
public class WorldObjectData
{
    public string id;
    public Vector3 position;
}

[System.Serializable]
public class SaveData
{
    public List<WorldObjectData> worldObjects = new List<WorldObjectData>();
    public List<string> inventorySaved;
    public List<Stock> stockSaved;
}


#endregion

#region SAVE SYSTEM

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static SaveData Load()
    {
        if (!File.Exists(path)) return null;
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}

#endregion

public class GameControlScript : MonoBehaviour
{
    public float time = 0f;
    public bool isDay = true;

    public CameraScript cameraScript;
    public InventoryScript inventoryScript;
    public StandScript standScript;

    public GameObject weedPrefab;
    public GameObject treePrefab;
    public GameObject zombiePrefab;

    private GameObject[] tiles;
    private string[] treeTypes = { "Tree4" };
    

    void Start()
    {
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        standScript = GameObject.Find("Stand").GetComponent<StandScript>();
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        StartCoroutine(DayNightCycle());
        StartCoroutine(Weeds());
        /*
        SaveData data = SaveSystem.Load();
        if (data != null)
            LoadWorld(data);
        else
        {
            Debug.Log("Data is null");
            SpawnWorld();        
        }
        */
        SpawnWorld();
    }

    void Update()
    {
        time += Time.deltaTime;

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            SaveSystem.Save(BuildSaveData());
            Debug.Log("World Saved");
        }
        /*if(Keyboard.current.pKey.wasPressedThisFrame)
        {
            SaveData data = SaveSystem.Load();
            LoadWorld(data);

        }*/
    }

    #region SAVE / LOAD

    SaveData BuildSaveData()
    {
        SaveData data = new SaveData();

        foreach (GameObject weed in GameObject.FindGameObjectsWithTag("Weed"))
        {
            data.worldObjects.Add(new WorldObjectData
            {
                id = "Weed",
                position = weed.transform.position
            });
        }

        foreach (GameObject tree in GameObject.FindGameObjectsWithTag("Tree"))
        {
            data.worldObjects.Add(new WorldObjectData
            {
                id = "Tree",
                position = tree.transform.position
            });
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
            GameObject prefab = null;

            if (obj.id == "Weed") prefab = weedPrefab;
            if (obj.id == "Tree") prefab = treePrefab;

            if (prefab != null)
            
                Instantiate(prefab, obj.position, Quaternion.identity);
        }

        inventoryScript.inventory = data.inventorySaved;
        inventoryScript.stocks = data.stockSaved;

        Debug.Log("World Loaded");
    }

    #endregion

    #region WORLD GEN

    void SpawnWorld()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject tile = tiles[Random.Range(0, tiles.Length)];
            TileScript ts = tile.GetComponent<TileScript>();

            if (!ts.isFull)
                cameraScript.Build(tile, weedPrefab, new Vector3(0, 0, 1), true);
        }

        for (int i = 0; i < 30; i++)
        {
            GameObject tile = tiles[Random.Range(0, tiles.Length)];
            TileScript ts = tile.GetComponent<TileScript>();

            if (!ts.isFull)
                cameraScript.Build(tile, treePrefab, new Vector3(0, 1f, 1), true, treeTypes);
        }
    }

    IEnumerator Weeds()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            GameObject tile = tiles[Random.Range(0, tiles.Length)];
            TileScript ts = tile.GetComponent<TileScript>();

            if (!ts.isFull)
                cameraScript.Build(tile, weedPrefab, new Vector3(0, 0, 1), true);
        }
    }

    IEnumerator DayNightCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            isDay = !isDay;

            if (!isDay)
                Instantiate(zombiePrefab, new Vector3(0, 0, 1), Quaternion.identity);
        }
    }

    #endregion
}

using System.Collections;
using UnityEngine;

public class TileGeneratorScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public CameraScript cameraScript;
    public GameObject tilePrefab;
    public GameObject waterPrefab;
    public int riverThickness = 1;
    public int gridWidth = 20;

    void Start()
    {
        int width = 40;
        int height = 40;
        int edgeThickness = 3;

        Sprite sandSprite = Resources.Load<Sprite>("Images/Sand");
        int index = 1;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.name = "Tile" + index;

                TileScript ts = tile.GetComponent<TileScript>();
                
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();

                ts.tileID = index;
                ts.isWater = false;

                bool isEdge =
                    x < edgeThickness ||
                    x >= width - edgeThickness ||
                    y < edgeThickness ||
                    y >= height - edgeThickness;

                if (isEdge)
                {
                    sr.sprite = sandSprite;
                    ts.isFertile = false;
                }
                else
                {
                    ts.isFertile = true;
                }

                index++;
            }
        }
        StartCoroutine(waterTiles());
    }
    //007AEC = Water Color
    //
    IEnumerator waterTiles()
    {
        yield return new WaitForSeconds(0.1f);

        foreach (GameObject tile in gameControlScript.tiles)
        {
            TileScript ts = tile.GetComponent<TileScript>();
            if (ts == null) continue;

            int x = ts.tileID % gridWidth;
            int y = ts.tileID / gridWidth + 20;

            float noise = Mathf.PerlinNoise(y * 0.08f, 0f);
            int riverX = Mathf.RoundToInt(noise * 10f) + (gridWidth / 2);

            int thickness =
                riverThickness + Mathf.RoundToInt(
                    Mathf.PerlinNoise(0f, y * 0.12f) * 0.5f
                );

            if (Mathf.Abs(x - riverX) <= thickness)
            {
                if (ts.isFull)
                {
                    Destroy(tile.transform.GetChild(0).gameObject);
                    ts.isFull = false;
                }

                cameraScript.Build(
                    tile,
                    waterPrefab,
                    new Vector3(0, 0, 1),
                    true
                );

                ts.isWater = true;
                ts.isFull = true;
            }
        }

        Debug.Log("Foreach works");
    }

}



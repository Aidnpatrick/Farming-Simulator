using UnityEngine;

public class TileGeneratorScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public CameraScript cameraScript;
    public GameObject tilePrefab;
    public GameObject waterPrefab;

    void Start()
    {
        int width = 40;
        int height = 40;
        int edgeThickness = 3;
        int waterDepth = 3;

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

        foreach (GameObject tile in gameControlScript.tiles)
        {
            
            Vector2 pos = tile.transform.position;
            TileScript ts = tile.GetComponent<TileScript>();

            if (pos.y < waterDepth)
            {
                Debug.Log("asdsad");
                cameraScript.Build(tile, waterPrefab, new Vector3(0, 0, 1), true);
                ts.isFertile = false;
            }
        }
    }
}



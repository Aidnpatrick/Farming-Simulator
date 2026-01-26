
using UnityEngine;

public class TileGeneratorScript : MonoBehaviour
{
    public GameObject tilePrefab;

    void Start()
    {
        int width = 40;
        int height = 40;
        int edgeThickness = 2;

        Sprite sandSprite = Resources.Load<Sprite>("Images/Sand");

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 pos = new Vector2(i, j);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.name = "Tile";

                bool isEdge =
                    i < edgeThickness ||
                    i >= width - edgeThickness ||
                    j < edgeThickness ||
                    j >= height - edgeThickness;

                if (isEdge)
                {
                    tile.GetComponent<SpriteRenderer>().sprite = sandSprite;
                }
            }
        }
    }


    void Update()
    {

    }
}

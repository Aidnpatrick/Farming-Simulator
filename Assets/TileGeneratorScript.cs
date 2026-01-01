
using UnityEngine;

public class TileGeneratorScript : MonoBehaviour
{
    public GameObject tilePrefab;

    
    void Start()
    {
        for(int i = 0; i < 40; i++)
        {
            for(int j = 0; j < 40; j++)
            {
                Vector2 pos = new Vector2(i,j);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.name = "Tile";
            }
        }
    }

    void Update()
    {

    }
}

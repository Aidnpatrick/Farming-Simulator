
using UnityEngine;

public class NPC : MonoBehaviour
{
    protected float speed = 1.2f;
    protected float health = 100f;
    protected float visionRange = 5f;

    protected Vector3 lastKnownLocation;
    protected bool canSeePlayer;

    protected float movingCooldown;
    protected float attackingCooldown;
    protected GameControlScript gameControlScript;
    protected GameObject targetCrop = null;

    void Start()
    {

        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        movingCooldown = 0f;
        attackingCooldown = 0f;
    }

    public virtual GameObject FindTargetCrop(GameObject[] tiles, bool needsWeed = false)
    {
        foreach (GameObject tile in tiles)
        {
            DirtScript ds = tile.GetComponentInChildren<DirtScript>();
            TileScript ts = tile.GetComponent<TileScript>();

            if (ds == null) continue;
            if (!ds.isFull) continue;

            if (Vector3.Distance(tile.transform.position, transform.position) < 8f)
            {
                targetCrop = tile;
                return targetCrop;
            }
        }
        Debug.Log("Cant Find tile");
        return null;
    }

    public virtual GameObject FindTargetWeed(GameObject[] tiles)
    {
        foreach (GameObject tile in tiles)
        {
            TileScript ts = tile.GetComponent<TileScript>();

            if(!ts.isFull) continue;
            if(!tile.transform.GetChild(0).name.Contains("Weed")) continue;

            if (Vector3.Distance(tile.transform.position, transform.position) < 4f)
            {
                targetCrop = tile;
                return targetCrop;
            }
        }
        Debug.Log("Cant Find tile");
        return null;
    }
    public virtual GameObject FindTargetAnimal(GameObject[] animals)
    {
        foreach (GameObject tile in animals)
        {
            if (Vector3.Distance(tile.transform.position, transform.position) < 5f)
            {
                targetCrop = tile;
                return targetCrop;
            }
        }
        Debug.Log("Cant Find tile");
        return null;
    }


    public virtual void MoveTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.z = 0f;

        if (direction.magnitude < 0.1f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        transform.position += direction.normalized * speed * Time.deltaTime;
    }

    public Vector3 FindRandomPos()
    {
        
        Vector2 offset = new Vector2(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)
        );
        if(offset.x < 0)           
            offset = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        return transform.position + (Vector3)offset;
    }
}

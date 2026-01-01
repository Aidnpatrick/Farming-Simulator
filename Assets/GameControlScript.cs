using UnityEngine;
using System.Collections;

public class GameControlScript : MonoBehaviour
{
    public float time = 0f;
    public bool isDay = true;
    public GameObject enemyPrefab;

    void Start()
    {
        StartCoroutine(DayNightCycle());
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

    IEnumerator SpawnZombies()
    {
        yield return new WaitForSeconds(1f);
    }
}


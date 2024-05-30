using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;    // Prefab to spawn
    public float spawnInterval = 10f;   // Interval between spawns
    public float startDelay = 10f;      // Delay before the first spawn

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(startDelay);

        // Continue to spawn at regular intervals
        while (true)
        {
            SpawnPrefab();
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    private void SpawnPrefab()
    {
        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}

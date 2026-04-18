using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSpawner : MonoBehaviour
{
    [Header("Base Spawner Settings")]
    public int maxEnemies = 5;
    public float initialDelay = 1f;
    public float spawnInterval = 0.5f;
    public bool isActive = true;

    protected List<GameObject> currentEnemies = new List<GameObject>();

    public virtual void BeginSpawning()
    {
        isActive = true;
        StartCoroutine(SpawnRoutine());
    }

    protected virtual IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (isActive)
        {
            currentEnemies.RemoveAll(item => item == null);

            if (currentEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    protected virtual GameObject GetEnemyToSpawn() { return null; }

    protected virtual void SpawnEnemy()
    {
        GameObject prefabToSpawn = GetEnemyToSpawn();
        if (prefabToSpawn == null) return;

        Vector3 spawnPoint = GetSpawnPosition();
        NavMeshHit hit;

        if (NavMesh.SamplePosition(spawnPoint, out hit, 5.0f, NavMesh.AllAreas))
        {
            GameObject newEnemy = Instantiate(prefabToSpawn, hit.position, Quaternion.identity);
            currentEnemies.Add(newEnemy);
        }
    }

    protected abstract Vector3 GetSpawnPosition();

    public void ClearEnemies()
    {
        foreach (var enemy in currentEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        currentEnemies.Clear();
    }
}
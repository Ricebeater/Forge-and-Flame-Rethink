using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSpawner : MonoBehaviour
{
    [Header("Base Spawner Settings")]
    public GameObject enemyPrefab;
    public int maxEnemies = 5;
    public float spawnInterval = 3f;
    public bool isActive = true;

    protected List<GameObject> currentEnemies = new List<GameObject>();

    protected virtual void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (isActive)
            {
                currentEnemies.RemoveAll(item => item == null);

                if (currentEnemies.Count < maxEnemies)
                {
                    SpawnEnemy();
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    protected virtual void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        Vector3 spawnPoint = GetSpawnPosition();
        NavMeshHit hit;

        if (NavMesh.SamplePosition(spawnPoint, out hit, 5.0f, NavMesh.AllAreas))
        {
            GameObject newEnemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            currentEnemies.Add(newEnemy);
        }
    }

    protected abstract Vector3 GetSpawnPosition();

    public virtual void ClearEnemies()
    {
        foreach (var enemy in currentEnemies)
        {
            if (enemy != null)
            {
                EnemyAI ai = enemy.GetComponent<EnemyAI>();
                if (ai != null) ai.StartFleeing();
                else Destroy(enemy);
            }
        }
        currentEnemies.Clear();
    }
}
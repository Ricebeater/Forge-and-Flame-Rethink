using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnSettings
{
    public string enemyName;
    public GameObject prefab;
    [Range(1, 10)] public int weight = 1;
}

public class AreaSpawner : BaseSpawner
{
    [Header("Enemies Configuration")]
    public List<EnemySpawnSettings> enemyPool = new List<EnemySpawnSettings>();

    [Header("Area Spawn Settings")]
    public Transform spawnCenter;
    [Range(1f, 100f)] public float areaRadius = 15f;

    protected override GameObject GetEnemyToSpawn()
    {
        if (enemyPool == null || enemyPool.Count == 0) return null;

        int totalWeight = 0;
        foreach (var enemy in enemyPool) totalWeight += enemy.weight;

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var enemy in enemyPool)
        {
            currentWeight += enemy.weight;
            if (randomValue < currentWeight) return enemy.prefab;
        }
        return null;
    }

    protected override Vector3 GetSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * areaRadius;
        Vector3 spawnOffset = new Vector3(randomCircle.x, 0, randomCircle.y);
        return (spawnCenter != null ? spawnCenter.position : transform.position) + spawnOffset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnCenter != null ? spawnCenter.position : transform.position, areaRadius);
    }
}
using UnityEngine;

public class AroundSpawner : BaseSpawner
{
    [Header("Around Spawn Settings")]
    public Transform target;
    public float spawnRadius = 20f;
    public float minSpawnDistance = 5f;

    private void Start()
    {
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    protected override Vector3 GetSpawnPosition()
    {
        if (target == null) return transform.position;

        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, spawnRadius);
        Vector3 spawnOffset = new Vector3(randomCircle.x, 0, randomCircle.y) * randomDistance;

        return target.position + spawnOffset;
    }

    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position, spawnRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, minSpawnDistance);
        }
    }
}
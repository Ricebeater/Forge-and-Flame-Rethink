using UnityEngine;

public class AreaSpawner : BaseSpawner
{
    [Header("Area Spawn Settings")]
    public Transform spawnCenter;
    public float areaRadius = 15f;

    protected override void Start()
    {
        if (spawnCenter == null) spawnCenter = transform;
        base.Start();
    }

    protected override Vector3 GetSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * areaRadius;
        Vector3 spawnOffset = new Vector3(randomCircle.x, 0, randomCircle.y);
        return spawnCenter.position + spawnOffset;
    }

    private void OnDrawGizmosSelected()
    {
        Transform center = spawnCenter != null ? spawnCenter : transform;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center.position, areaRadius);
    }
}
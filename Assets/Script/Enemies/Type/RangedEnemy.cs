using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Range Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void PerformAttack()
    {
        base.PerformAttack();
        if (projectilePrefab != null && firePoint != null && player != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                ep.Setup(dir, damage, projectileSpeed);
            }
        }
    }
}
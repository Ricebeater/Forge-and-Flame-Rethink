using UnityEngine;

public class AttackState : EnemyBaseState
{
    float attackTimer;

    public override void EnterState(EnemyAI ai)
    {
        ai.enemy.agent.isStopped = true;
        ai.enemy.agent.velocity = Vector3.zero;
        attackTimer = 0f;
    }

    public override void UpdateState(EnemyAI ai)
    {
        if (ai.enemy.player == null) return;

        ai.transform.LookAt(ai.enemy.player);

        if (Vector3.Distance(ai.transform.position, ai.enemy.player.position) > ai.enemy.attackRange)
        {
            ai.ChangeState(ai.chaseState);
            return;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= ai.enemy.attackCooldown)
        {
            attackTimer = 0f;
            PerformAttack(ai);
        }
    }

    public override void ExitState(EnemyAI ai)
    {
        ai.enemy.agent.isStopped = false;
    }

    void PerformAttack(EnemyAI ai)
    {
        ai.enemy.PlayAttackAnimation();
        if (ai.enemy.isRanged)
        {
            if (ai.enemy.projectilePrefab != null && ai.enemy.firePoint != null)
            {
                Vector3 targetPos;
                Collider playerCol = ai.enemy.player.GetComponent<Collider>();
                if (playerCol != null) targetPos = playerCol.bounds.center;
                else targetPos = ai.enemy.player.position + Vector3.up;

                Vector3 dirToPlayer = (targetPos - ai.enemy.firePoint.position).normalized;

                GameObject bulletObj = GameObject.Instantiate(
                    ai.enemy.projectilePrefab,
                    ai.enemy.firePoint.position,
                    Quaternion.LookRotation(dirToPlayer)
                );

                EnemyProjectile projectile = bulletObj.GetComponent<EnemyProjectile>();
                if (projectile != null)
                {
                    projectile.Setup(dirToPlayer, ai.enemy.damage, ai.enemy.projectileSpeed);
                }
            }
        }
        else
        {
            Character targetChar = ai.enemy.player.GetComponent<Character>();
            if (targetChar != null)
            {
                targetChar.TakeDamage(ai.enemy.damage);
            }
        }
    }
}
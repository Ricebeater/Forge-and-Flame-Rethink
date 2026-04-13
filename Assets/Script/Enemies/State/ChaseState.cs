using UnityEngine;

public class ChaseState : EnemyBaseState
{
    public override void EnterState(EnemyAI ai)
    {
        ai.enemy.agent.speed = ai.enemy.chaseSpeed;
        ai.enemy.agent.isStopped = false;
        ai.enemy.agent.stoppingDistance = 0f;
    }

    public override void UpdateState(EnemyAI ai)
    {
        if (ai.enemy.player == null) return;

        ai.enemy.agent.SetDestination(ai.enemy.player.position);

        float distance = Vector3.Distance(ai.transform.position, ai.enemy.player.position);

        if (!(ai.enemy is BossEnemy) && distance > ai.enemy.chaseRange)
        {
            ai.ChangeState(ai.patrolState);
        }

        float stopDistance = ai.enemy.attackRange;

        if (ai.enemy is RangedEnemy)
        {
            stopDistance = ai.enemy.attackRange * 0.8f;
        }

        if (distance <= stopDistance)
        {
            if (ai.enemy is BossEnemy boss)
            {
                if (Time.time >= boss.lastBossSkillTime + boss.bossSkillCooldown)
                {
                    ai.ChangeState(ai.bossAttackState);
                }
                else
                {
                    ai.ChangeState(ai.attackState);
                }
            }
            else
            {
                ai.ChangeState(ai.attackState);
            }
        }
    }

    public override void ExitState(EnemyAI ai)
    {

    }
}
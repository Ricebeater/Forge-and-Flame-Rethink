using UnityEngine;

public class IdleState : EnemyBaseState
{
    public override void EnterState(EnemyAI ai)
    {
        ai.enemy.agent.isStopped = true;
        ai.enemy.agent.velocity = Vector3.zero;
    }

    public override void UpdateState(EnemyAI ai)
    {
        if (ai.enemy.player == null) return;

        float distance = Vector3.Distance(ai.transform.position, ai.enemy.player.position);

        float stopDistance = ai.enemy.profile.attackRange;
        if (ai.enemy is RangedEnemy) stopDistance = ai.enemy.profile.attackRange * 0.8f;

        if (distance <= stopDistance)
        {
            ai.ChangeState(ai.attackState);
            return;
        }

        if (distance < ai.enemy.profile.chaseRange)
        {
            ai.ChangeState(ai.chaseState);
        }
    }

    public override void ExitState(EnemyAI ai)
    {
        ai.enemy.agent.isStopped = false;
    }
}
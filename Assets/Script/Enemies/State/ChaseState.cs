using UnityEngine;

public class ChaseState : EnemyBaseState
{
    public override void EnterState(EnemyAI ai)
    {
        if (ai.enemy.agent.isActiveAndEnabled && ai.enemy.agent.isOnNavMesh)
        {
            ai.enemy.agent.speed = ai.enemy.profile.chaseSpeed;
            ai.enemy.agent.isStopped = false;
            ai.enemy.agent.stoppingDistance = 0f;
        }
    }

    public override void UpdateState(EnemyAI ai)
    {
        if (ai.enemy.isAnimatingAttack) return;

        if (ai.enemy.player == null) return;

        // เช็กก่อนสั่งให้เดินไปหาผู้เล่น
        if (ai.enemy.agent.isActiveAndEnabled && ai.enemy.agent.isOnNavMesh)
        {
            ai.enemy.agent.SetDestination(ai.enemy.player.position);
        }

        float distance = Vector3.Distance(ai.transform.position, ai.enemy.player.position);

        if (!(ai.enemy is BossEnemy) && distance > ai.enemy.profile.chaseRange)
        {
            ai.ChangeStateByOption(ai.enemy.profile.onLostPlayer);
            return;
        }

        float stopDistance = ai.enemy.profile.attackRange;
        if (ai.enemy is RangedEnemy) stopDistance = ai.enemy.profile.attackRange * 0.8f;

        if (distance <= stopDistance) ai.ChangeState(ai.attackState);
    }

    public override void ExitState(EnemyAI ai) { }
}
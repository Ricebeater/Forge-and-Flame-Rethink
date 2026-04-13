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


        Vector3 targetPosition = new Vector3(
            ai.enemy.player.position.x,
            ai.transform.position.y,
            ai.enemy.player.position.z
        );

        ai.transform.LookAt(targetPosition);

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
        ai.enemy.PerformAttack();

        if (!(ai.enemy is RangedEnemy))
        {
            Character targetChar = ai.enemy.player.GetComponent<Character>();
            if (targetChar != null)
            {
                targetChar.TakeDamage(ai.enemy.damage);
            }
        }
    }
}
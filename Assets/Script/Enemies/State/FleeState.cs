using UnityEngine;

public class FleeState : EnemyBaseState
{
    float destroyTimer = 0f;
    float timeToDespawn = 5f;

    public override void EnterState(EnemyAI ai)
    {
        ai.enemy.agent.speed = ai.enemy.profile.chaseSpeed * 1.5f;
        ai.enemy.agent.isStopped = false;

        ai.enemy.agent.stoppingDistance = 0f;
    }

    public override void UpdateState(EnemyAI ai)
    {
        if (ai.enemy.player == null) return;

        Vector3 dirToPlayer = ai.transform.position - ai.enemy.player.position;
        Vector3 newPos = ai.transform.position + dirToPlayer.normalized * 5f;

        ai.enemy.agent.SetDestination(newPos);

        destroyTimer += Time.deltaTime;
        if (destroyTimer >= timeToDespawn)
        {
            GameObject.Destroy(ai.gameObject);
        }
    }

    public override void ExitState(EnemyAI ai)
    {

    }
}
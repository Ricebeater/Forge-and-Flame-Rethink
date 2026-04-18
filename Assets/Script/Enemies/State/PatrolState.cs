using UnityEngine;

public class PatrolState : EnemyBaseState
{
    float timer;
    float waitTime;

    public override void EnterState(EnemyAI ai)
    {
        ai.enemy.agent.speed = ai.enemy.profile.patrolSpeed;
        SetRandomDestination(ai);
    }

    public override void UpdateState(EnemyAI ai)
    {
        if (ai.enemy.player == null) return;
        if (!ai.enemy.agent.pathPending && ai.enemy.agent.remainingDistance < 0.5f)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime) SetRandomDestination(ai);
        }

        if (Vector3.Distance(ai.transform.position, ai.enemy.player.position) < ai.enemy.profile.chaseRange)
            ai.ChangeState(ai.chaseState);
    }

    public override void ExitState(EnemyAI ai) { }

    void SetRandomDestination(EnemyAI ai)
    {
        timer = 0;
        waitTime = Random.Range(ai.enemy.profile.minWaitTime, ai.enemy.profile.maxWaitTime);
        Vector3 newPos;
        if (ai.RandomPoint(ai.transform.position, ai.enemy.profile.patrolRadius, out newPos))
            ai.enemy.agent.SetDestination(newPos);
    }
}
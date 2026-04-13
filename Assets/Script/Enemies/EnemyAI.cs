using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour
{
    public Enemy enemy;

    private EnemyBaseState currentState;
    public readonly PatrolState patrolState = new PatrolState();
    public readonly ChaseState chaseState = new ChaseState();
    public readonly AttackState attackState = new AttackState();
    public readonly BossChargeAttackState bossAttackState = new BossChargeAttackState();
    public readonly FleeState fleeState = new FleeState();

    private void Reset()
    {
        enemy = GetComponent<Enemy>();
    }

    void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }
    }

    void Start()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError($"[EnemyAI] {name}: Critical Error! Not found Script 'Enemy' Stop working");
            this.enabled = false;
            return;
        }

        if (enemy.agent == null)
        {
            enemy.agent = GetComponent<NavMeshAgent>();
        }

        if (enemy is BossEnemy)
        {
            ChangeState(chaseState);
        }
        else
        {
            ChangeState(patrolState);
        }
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public void ChangeState(EnemyBaseState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
    }

    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }
    public void StartFleeing()
    {
        if (enemy is BossEnemy)
        {
            Destroy(gameObject);
            return;
        }

        ChangeState(fleeState);
    }
}
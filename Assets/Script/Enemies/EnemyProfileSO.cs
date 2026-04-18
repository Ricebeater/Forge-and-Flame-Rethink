using UnityEngine;

public enum AIStateOption
{
    Idle,
    Patrol,
    Chase,
    Flee
}

[CreateAssetMenu(fileName = "NewEnemyProfile", menuName = "Enemy Profile")]
public class EnemyProfileSO : ScriptableObject
{
    [Header("Identity")]
    public string enemyName = "Enemy";

    [Header("AI Behavior Settings")]
    [Tooltip("First state that enemy will do")]
    public AIStateOption startingState = AIStateOption.Patrol;

    [Tooltip("If player out of sight")]
    public AIStateOption onLostPlayer = AIStateOption.Patrol;
    [Header("Movement Stats")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float chaseRange = 8f;
    public float patrolRadius = 5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    [Header("Combat Stats")]
    public int maxHP = 100;
    public int damage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    [Header("Special States (Flee)")]
    public float fleeDistance = 5f;
    public float despawnTime = 5f;
}
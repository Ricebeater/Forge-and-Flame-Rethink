using UnityEngine;

public class NormalEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        maxHP = 4;
        damage = 1;
        patrolSpeed = 2f;
        chaseSpeed = 2f;
    }
}
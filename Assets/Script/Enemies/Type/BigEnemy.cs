using UnityEngine;

public class BigEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        maxHP = 7;
        damage = 2;
        patrolSpeed = 1f;
        chaseSpeed = 1.5f;
    }
}
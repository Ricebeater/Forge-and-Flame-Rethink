using UnityEngine;

public class SpeedsterEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        maxHP = 2;
        damage = 1;
        patrolSpeed = 3f;
        chaseSpeed = 4f;
    }
}
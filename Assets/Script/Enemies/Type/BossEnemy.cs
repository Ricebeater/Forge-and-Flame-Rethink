using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("Boss Specific Settings")]
    public GameObject skillEffect;
    public GameObject attackIndicatorPrefab;
    public SoundProfile bossChargeSound;
    public SoundProfile bossExplosionSound;
    public float bossChargeTime = 2f;
    public float bossAttackRadius = 3f;
    public float bossSkillCooldown = 5f;

    [HideInInspector] public float lastBossSkillTime = -999f;

    protected override void Awake()
    {
        base.Awake();
        maxHP = 50;
        damage = 5;
        patrolSpeed = 0f;
        chaseSpeed = 4f;
    }

    protected override void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.isWin = true;
            GameManager.Instance.TriggerEndGame();
        }
        base.Die();
    }
}
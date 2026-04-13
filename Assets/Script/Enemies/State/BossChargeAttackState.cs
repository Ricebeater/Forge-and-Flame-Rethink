using UnityEngine;

public class BossChargeAttackState : EnemyBaseState
{
    float chargeTimer;
    GameObject currentIndicator;
    GameObject currentSkillEffect;
    Vector3 attackPosition;

    public override void EnterState(EnemyAI ai)
    {
        BossEnemy boss = ai.enemy as BossEnemy;
        if (boss == null) return;

        boss.agent.isStopped = true;
        boss.agent.velocity = Vector3.zero;
        chargeTimer = 0f;

        Vector3 castOrigin = boss.player.position + Vector3.up;
        RaycastHit hit;

        if (Physics.Raycast(castOrigin, Vector3.down, out hit, 20f, boss.groundLayer))
        {
            attackPosition = hit.point + (Vector3.up * 0.05f);
        }
        else
        {
            attackPosition = new Vector3(boss.player.position.x, ai.transform.position.y + 0.05f, boss.player.position.z);
        }

        ai.transform.LookAt(new Vector3(attackPosition.x, ai.transform.position.y, attackPosition.z));

        if (boss.attackIndicatorPrefab != null)
        {
            currentIndicator = GameObject.Instantiate(boss.attackIndicatorPrefab, attackPosition, Quaternion.identity);
        }

        if (SoundManager.Instance != null && boss.bossChargeSound != null)
            SoundManager.Instance.PlaySFX(boss.bossChargeSound);
    }

    public override void UpdateState(EnemyAI ai)
    {
        BossEnemy boss = ai.enemy as BossEnemy;
        if (boss == null) return;

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= boss.bossChargeTime)
        {
            PerformBossAttack(ai, boss);
        }
    }

    public override void ExitState(EnemyAI ai)
    {
        ai.enemy.agent.isStopped = false;
        if (currentIndicator != null) GameObject.Destroy(currentIndicator);
        if (currentSkillEffect != null) GameObject.Destroy(currentSkillEffect);
    }

    void PerformBossAttack(EnemyAI ai, BossEnemy boss)
    {
        if (currentIndicator != null)
        {
            GameObject.Destroy(currentIndicator);
        }
        if (currentSkillEffect != null)
        {
            GameObject.Destroy(currentSkillEffect);
        }

        if (boss.skillEffect != null)
        {
            Vector3 skillEffectPosition = new Vector3(attackPosition.x, (attackPosition.y * 10f), attackPosition.z);

            if (SoundManager.Instance != null && boss.bossExplosionSound != null)
                SoundManager.Instance.PlaySFX(boss.bossExplosionSound);

            GameObject skillVFX = GameObject.Instantiate(boss.skillEffect, skillEffectPosition, Quaternion.identity);
            GameObject.Destroy(skillVFX, 3f);
        }

        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, boss.bossAttackRadius, boss.targetLayer);

        foreach (var hitCollider in hitColliders)
        {
            Character target = hitCollider.GetComponent<Character>();
            if (target != null)
            {
                target.TakeDamage(boss.damage + 10);
            }
        }

        boss.lastBossSkillTime = Time.time;
        ai.ChangeState(ai.chaseState);
    }
}
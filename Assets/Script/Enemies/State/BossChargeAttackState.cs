using UnityEngine;

public class BossChargeAttackState : EnemyBaseState
{
    float chargeTimer;
    GameObject currentIndicator;
    GameObject currentSkillEffect;
    Vector3 attackPosition;

    public override void EnterState(EnemyAI ai)
    {
        ai.enemy.agent.isStopped = true;
        ai.enemy.agent.velocity = Vector3.zero;
        chargeTimer = 0f;

        Vector3 castOrigin = ai.enemy.player.position + Vector3.up;
        RaycastHit hit;

        if (Physics.Raycast(castOrigin, Vector3.down, out hit, 20f, ai.enemy.groundLayer))
        {
            attackPosition = hit.point + (Vector3.up * 0.05f);
        }
        else
        {
            attackPosition = new Vector3(ai.enemy.player.position.x, ai.transform.position.y + 0.05f, ai.enemy.player.position.z);
        }

        ai.transform.LookAt(new Vector3(attackPosition.x, ai.transform.position.y, attackPosition.z));

        if (ai.enemy.attackIndicatorPrefab != null)
        {
            currentIndicator = GameObject.Instantiate(ai.enemy.attackIndicatorPrefab, attackPosition, Quaternion.identity);
        }

        SoundManager.Instance.PlaySFX(ai.enemy.bossChargeSound);
    }

    public override void UpdateState(EnemyAI ai)
    {
        chargeTimer += Time.deltaTime;

        if (chargeTimer >= ai.enemy.bossChargeTime)
        {
            PerformBossAttack(ai);
        }
    }

    public override void ExitState(EnemyAI ai)
    {
        ai.enemy.agent.isStopped = false;
        if (currentIndicator != null) GameObject.Destroy(currentIndicator);
        if (currentSkillEffect != null) GameObject.Destroy(currentSkillEffect);
    }

    void PerformBossAttack(EnemyAI ai)
    {
        if (currentIndicator != null)
        {
            GameObject.Destroy(currentIndicator);
        }
        if (ai.enemy.skillEffect != null)
        {
            GameObject.Destroy(currentSkillEffect);
        }

        if (ai.enemy.skillEffect != null)
        {
            Vector3 skillEffectPosition = new Vector3(attackPosition.x, (attackPosition.y * 10f), attackPosition.z);
            SoundManager.Instance.PlaySFX(ai.enemy.bossExplosionSound);
            GameObject skillVFX = GameObject.Instantiate(ai.enemy.skillEffect, skillEffectPosition, Quaternion.identity);
            GameObject.Destroy(skillVFX, 3f);
        }
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, ai.enemy.bossAttackRadius, ai.enemy.targetLayer);

        foreach (var hitCollider in hitColliders)
        {
            Character target = hitCollider.GetComponent<Character>();
            if (target != null)
            {
                target.TakeDamage(ai.enemy.damage + 10);
            }
        }
        ai.enemy.lastBossSkillTime = Time.time;
        ai.ChangeState(ai.chaseState);
    }
}
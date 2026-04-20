using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BigEnemy : Enemy
{
    [Header("Big Enemy - Stomp Attack")]
    public float jumpHeight = 3f;
    public float jumpDuration = 0.8f;
    public float stompDamageRadius = 2.5f;

    [Header("Big Enemy - Audio")]
    [Tooltip("Impact sound")]
    public SoundProfile stompSound;

    private Vector3 originalVisualLocalPos;

    protected override void Start()
    {
        base.Start();
        if (visualRoot != null)
            originalVisualLocalPos = visualRoot.localPosition;
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);

        if (visualRoot != null)
        {
            visualRoot.localPosition = originalVisualLocalPos;
        }

        if (!agent.enabled && gameObject.activeInHierarchy)
        {
            agent.enabled = true;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    public override void PerformAttack()
    {
        if (isAnimatingAttack) return;

        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(JumpStompRoutine());
    }

    private IEnumerator JumpStompRoutine()
    {
        isAnimatingAttack = true;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.isStopped = true;

        float timer = 0;
        Vector3 squatScale = new Vector3(originalScale.x * 1.4f, originalScale.y * 0.6f, originalScale.z * 1.4f);

        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            visualRoot.localScale = Vector3.Lerp(originalScale, squatScale, timer / 0.3f);
            yield return null;
        }

        if (SoundManager.Instance != null && audioSettings.attackSounds != null)
            SoundManager.Instance.PlayRandomSFX(audioSettings.attackSounds);

        Vector3 targetPos = player.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 3f, NavMesh.AllAreas))
        {
            targetPos = hit.position;
        }

        if (agent.isActiveAndEnabled) agent.enabled = false;

        Vector3 startPos = transform.position;

        timer = 0;
        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / jumpDuration;

            transform.position = Vector3.Lerp(startPos, targetPos, percent);

            float height = Mathf.Sin(Mathf.PI * percent) * jumpHeight;
            visualRoot.localPosition = originalVisualLocalPos + new Vector3(0, height, 0);

            visualRoot.localScale = Vector3.Lerp(squatScale, new Vector3(originalScale.x * 0.7f, originalScale.y * 1.3f, originalScale.z * 0.7f), percent * 2f);

            yield return null;
        }

        visualRoot.localPosition = originalVisualLocalPos;
        transform.position = targetPos;

        agent.enabled = true;
        agent.Warp(targetPos);

        if (SoundManager.Instance != null && stompSound != null)
        {
            SoundManager.Instance.PlayRandomSFX(stompSound);

        }

        yield return null;

        timer = 0;
        Vector3 impactSquash = new Vector3(originalScale.x * 1.6f, originalScale.y * 0.4f, originalScale.z * 1.6f);
        while (timer < 0.15f)
        {
            timer += Time.deltaTime;
            visualRoot.localScale = Vector3.Lerp(visualRoot.localScale, impactSquash, timer / 0.15f);
            yield return null;
        }

        timer = 0;
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            visualRoot.localScale = Vector3.Lerp(impactSquash, originalScale, timer / 0.3f);
            yield return null;
        }

        visualRoot.localScale = originalScale;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.isStopped = false;

        isAnimatingAttack = false;
    }
}
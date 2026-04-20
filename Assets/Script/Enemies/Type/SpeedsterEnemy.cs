using System.Collections;
using UnityEngine;

[System.Serializable]
public class SpeedsterAnimSettings
{
    [Tooltip("Drag sprite to this")]
    public Transform wheelSprite;
    [Tooltip("Normal roll speed")]
    public float rollSpeedMultiplier = 300f;

    [Header("Dash Attack")]
    public float dashSpeed = 15f;        
    public float dashDuration = 0.5f;    
    public float anticipationTime = 0.6f;
}

public class SpeedsterEnemy : Enemy
{
    [Header("Speedster Specific Animation")]
    public SpeedsterAnimSettings speedsterAnim;

    protected override void HandleWalkAnimation()
    {
        base.HandleWalkAnimation();

        if (speedsterAnim.wheelSprite != null && !isAnimatingAttack)
        {
            if (agent.velocity.magnitude > 0.1f)
            {
                float direction = agent.velocity.x >= 0 ? -1f : 1f;
                float rotationAmount = direction * agent.velocity.magnitude * speedsterAnim.rollSpeedMultiplier * Time.deltaTime;
                speedsterAnim.wheelSprite.Rotate(0, 0, rotationAmount);
            }
            else
            {
                float currentZ = speedsterAnim.wheelSprite.localEulerAngles.z;
                if (currentZ > 180) currentZ -= 360;
                float newZ = Mathf.Lerp(currentZ, 0f, Time.deltaTime * 5f);
                speedsterAnim.wheelSprite.localRotation = Quaternion.Euler(speedsterAnim.wheelSprite.localEulerAngles.x, speedsterAnim.wheelSprite.localEulerAngles.y, newZ);
            }
        }
    }

    public override void PerformAttack()
    {
        if (isAnimatingAttack) return;

        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(DashAttackRoutine());
    }

    private IEnumerator DashAttackRoutine()
    {
        isAnimatingAttack = true;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.isStopped = true;

        float timer = 0;
        float faceDirection = player.position.x > transform.position.x ? -1f : 1f;

        while (timer < speedsterAnim.anticipationTime)
        {
            timer += Time.deltaTime;
            if (speedsterAnim.wheelSprite != null)
            {
                speedsterAnim.wheelSprite.Rotate(0, 0, faceDirection * -800f * Time.deltaTime);
            }
            visualRoot.localScale = Vector3.Lerp(originalScale, new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z * 1.2f), timer / speedsterAnim.anticipationTime);
            yield return null;
        }

        if (SoundManager.Instance != null && audioSettings.attackSounds != null)
            SoundManager.Instance.PlayRandomSFX(audioSettings.attackSounds);

        float originalSpeed = agent.speed;
        float originalAccel = agent.acceleration;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.speed = speedsterAnim.dashSpeed;
            agent.acceleration = 100f;
            agent.SetDestination(player.position);
        }

        timer = 0;
        while (timer < speedsterAnim.dashDuration)
        {
            timer += Time.deltaTime;
            visualRoot.localScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z * 0.8f);

            if (speedsterAnim.wheelSprite != null)
            {
                speedsterAnim.wheelSprite.Rotate(0, 0, faceDirection * 1500f * Time.deltaTime);
            }
            yield return null;
        }

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.speed = originalSpeed;
            agent.acceleration = originalAccel;

            if (profile != null) agent.speed = profile.patrolSpeed;

            agent.SetDestination(transform.position);
        }

        visualRoot.localScale = originalScale;
        isAnimatingAttack = false;
    }
}
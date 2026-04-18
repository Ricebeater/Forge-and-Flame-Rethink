using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Loot
{
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropChance;
}

[System.Serializable]
public class EnemyAnimationSettings
{
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.1f;
    public float walkSwayAmount = 5f;
}

[System.Serializable]
public class EnemyAudioSettings
{
    public SoundProfile attackSounds;
    public SoundProfile hurtSound;
    public SoundProfile dieSound;

    [Header("Footstep Settings")]
    public AudioClip[] footstepSounds;
    [Range(0f, 1f)] public float footstepVolume = 0.4f;
}

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character
{
    private AudioSource footstepSource;
    private float stepTimer = 0f;

    [HideInInspector] public Transform player;
    [HideInInspector] public LayerMask groundLayer;
    [HideInInspector] public NavMeshAgent agent;

    [Header("Enemy Data Profile")]
    public EnemyProfileSO profile;

    [Space(10)]
    public EnemyAnimationSettings animSettings;
    public EnemyAudioSettings audioSettings;

    [Header("Loot Settings")]
    public List<Loot> lootTable;

    protected Vector3 originalScale;
    protected Coroutine animationRoutine;
    protected bool isAnimatingAttack = false;

    protected override void Awake()
    {
        base.Awake();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        footstepSource = GetComponent<AudioSource>();

        if (profile != null)
        {
            maxHP = profile.maxHP;
            damage = profile.damage;
            agent.speed = profile.patrolSpeed;
        }
    }

    protected override void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (groundLayer.value == 0) groundLayer = LayerMask.GetMask("Ground", "Default");

        originalScale = transform.localScale;

        if (profile != null)
        {
            maxHP = profile.maxHP;
            currentHP = profile.maxHP;
            damage = profile.damage;
            agent.speed = profile.patrolSpeed;
        }

        base.Start();
    }

    void Update() { if (currentHP > 0 && !isAnimatingAttack) HandleWalkAnimation(); }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        if (SoundManager.Instance != null && audioSettings.hurtSound != null)
            SoundManager.Instance.PlayRandomSFX(audioSettings.hurtSound);

        if (animationRoutine != null) StopCoroutine(animationRoutine);
        transform.localScale = originalScale;
        isAnimatingAttack = false;
        StartCoroutine(HurtAnimation());
    }

    protected override void Die()
    {
        if (SoundManager.Instance != null && audioSettings.dieSound != null)
            SoundManager.Instance.PlaySFX(audioSettings.dieSound);
        base.Die();
        DropLoot();
        Destroy(gameObject);
    }

    public virtual void PerformAttack() { PlayAttackAnimation(); }

    public void PlayAttackAnimation()
    {
        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(BouncyAnimation());
        if (SoundManager.Instance != null && audioSettings.attackSounds != null)
            SoundManager.Instance.PlayRandomSFX(audioSettings.attackSounds);
    }

    protected void HandleWalkAnimation()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            float timer = Time.time * animSettings.walkBobSpeed;
            float bobValue = Mathf.Sin(timer) * animSettings.walkBobAmount;
            Vector3 targetScale = new Vector3(originalScale.x * (1 - bobValue), originalScale.y * (1 + bobValue), originalScale.z * (1 - bobValue));
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);

            float swayValue = Mathf.Cos(timer) * animSettings.walkSwayAmount;
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, swayValue);

            if (player != null && Vector3.Distance(transform.position, player.position) < 15f)
            {
                stepTimer -= Time.deltaTime;
                if (stepTimer <= 0f && footstepSource != null && audioSettings.footstepSounds.Length > 0)
                {
                    AudioClip clip = audioSettings.footstepSounds[Random.Range(0, audioSettings.footstepSounds.Length)];
                    footstepSource.pitch = Random.Range(0.85f, 1.15f);
                    footstepSource.PlayOneShot(clip, audioSettings.footstepVolume);

                    stepTimer = (Mathf.PI / animSettings.walkBobSpeed);
                }
            }
        }
        else
        {
            stepTimer = 0f;
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0), Time.deltaTime * 5f);
        }
    }

    protected virtual void DropLoot()
    {
        foreach (Loot loot in lootTable)
        {
            if (Random.Range(0f, 100f) <= loot.dropChance && loot.itemPrefab != null)
            {
                Vector3 spawnPos = transform.position;
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 10f, groundLayer))
                    spawnPos = hit.point + (Vector3.up * 0.2f);
                Instantiate(loot.itemPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    protected IEnumerator BouncyAnimation()
    {
        isAnimatingAttack = true;
        float timer = 0, speed = 0.1f;
        Vector3 squashScale = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z * 1.3f);
        while (timer < 1f) { timer += Time.deltaTime / speed; transform.localScale = Vector3.Lerp(originalScale, squashScale, timer); yield return null; }
        timer = 0;
        Vector3 stretchScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.4f, originalScale.z * 0.7f);
        while (timer < 1f) { timer += Time.deltaTime / speed; transform.localScale = Vector3.Lerp(squashScale, stretchScale, timer); yield return null; }
        timer = 0; float recoverySpeed = 0.2f;
        while (timer < 1f) { timer += Time.deltaTime / recoverySpeed; transform.localScale = Vector3.Lerp(stretchScale, originalScale, Mathf.SmoothStep(0f, 1f, timer)); yield return null; }
        transform.localScale = originalScale;
        isAnimatingAttack = false;
    }

    protected IEnumerator HurtAnimation()
    {
        Vector3 hurtScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.6f, originalScale.z * 1.2f);
        float timer = 0, speed = 0.1f;
        while (timer < 1f) { timer += Time.deltaTime / speed; transform.localScale = Vector3.Lerp(originalScale, hurtScale, timer); yield return null; }
        timer = 0;
        while (timer < 1f) { timer += Time.deltaTime / speed; transform.localScale = Vector3.Lerp(hurtScale, originalScale, timer); yield return null; }
        transform.localScale = originalScale;
    }
}
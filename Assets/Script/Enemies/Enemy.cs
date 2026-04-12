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

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character
{
    [Header("Shared Components")]
    public Transform player;
    public LayerMask targetLayer;
    public LayerMask groundLayer;
    [HideInInspector] public NavMeshAgent agent;
    
    [Header("Audio")]
    public SoundProfile attackSounds;
    public SoundProfile hurtSound;
    public SoundProfile dieSound;

    [Header("Walk Animation Settings")]
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.1f;
    public float walkSwayAmount = 5f;

    [Header("Movement Stats")]
    public float chaseRange = 8f;
    public float patrolRadius = 5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;

    [Header("Combat Stats")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    [Header("Range Enemy Settings")]
    public bool isRanged = false;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;

    [Header("Boss Settings")]
    public bool isBoss = false;
    public GameObject skillEffect;
    public GameObject attackIndicatorPrefab;
    public SoundProfile bossChargeSound;
    public SoundProfile bossExplosionSound;
    public float bossChargeTime = 2f;
    public float bossAttackRadius = 3f;
    public float bossSkillCooldown = 5f;
    [HideInInspector] public float lastBossSkillTime = -999f;

    [Header("Loot Settings")]
    public List<Loot> lootTable;

    private Vector3 originalScale;
    private Coroutine animationRoutine;
    private bool isAnimatingAttack = false;
    private void Reset()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (lootTable == null)
        {
            lootTable = new List<Loot>();
        }

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }
    protected override void Start()
    {
        base.Start();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p == null)
            {
                Player playerScript = FindFirstObjectByType<Player>();
                if (playerScript != null) 
                {
                    p = playerScript.gameObject;
                }
            }

            if (p != null)
            {
                player = p.transform;
            }
            else
            {
                Debug.LogWarning($"[Enemy] {name}: Not found Player! Check Tag or Inspector");
            }
               
        }

        if (groundLayer.value == 0)
        {
            Debug.LogWarning($"[Enemy] {name}: Forget 'Ground Layer' the Loot/Boss system May not work perfecly");
            groundLayer = LayerMask.GetMask("Default");
        }

        originalScale = transform.localScale;
    }
    void Update()
    {
        if (currentHP <= 0 || isAnimatingAttack) return;

        HandleWalkAnimation();
    }
    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);

        SoundManager.Instance.PlayRandomSFX(hurtSound);

        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }

        if (originalScale != Vector3.zero)
        {
            transform.localScale = originalScale;
        }
        isAnimatingAttack = false;
        StartCoroutine(HurtAnimation());
    }

    protected override void Die()
    {
        SoundManager.Instance.PlaySFX(dieSound);

        base.Die();
        DropLoot();
        if (isBoss)
        {
            if (GameManager.Instance != null)
            {
                GameManager.isWin = true;
                GameManager.Instance.TriggerEndGame();
            }
        }
        Destroy(gameObject);
    }

    void DropLoot()
    {
        foreach (Loot loot in lootTable)
        {
            float randomValue = Random.Range(0f, 100f);

            if (randomValue <= loot.dropChance)
            {
                Vector3 spawnPos = transform.position;
                RaycastHit hit;

                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10f, groundLayer))
                {
                    spawnPos = hit.point + (Vector3.up * 0.2f);
                }

                if (loot.itemPrefab != null)
                {
                    Instantiate(loot.itemPrefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }

    public void PlayAttackAnimation()
    {
        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(BouncyAnimation());

        if (SoundManager.Instance != null && attackSounds != null) // เช็ค null กันพลาด
            SoundManager.Instance.PlayRandomSFX(attackSounds);
    }

    IEnumerator BouncyAnimation()
    {
        isAnimatingAttack = true;

        float timer = 0;
        float speed = 0.1f;

        Vector3 squashScale = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z * 1.3f);

        while (timer < 1f)
        {
            timer += Time.deltaTime / speed;
            transform.localScale = Vector3.Lerp(originalScale, squashScale, timer);
            yield return null;
        }

        timer = 0;
        Vector3 stretchScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.4f, originalScale.z * 0.7f);

        while (timer < 1f)
        {
            timer += Time.deltaTime / speed;
            transform.localScale = Vector3.Lerp(squashScale, stretchScale, timer);
            yield return null;
        }

        timer = 0;
        float recoverySpeed = 0.2f;

        while (timer < 1f)
        {
            timer += Time.deltaTime / recoverySpeed;
            transform.localScale = Vector3.Lerp(stretchScale, originalScale, Mathf.SmoothStep(0f, 1f, timer));
            yield return null;
        }

        transform.localScale = originalScale;
        isAnimatingAttack = false;
    }

    IEnumerator HurtAnimation()
    {
        Vector3 hurtScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.6f, originalScale.z * 1.2f);

        float timer = 0;
        float speed = 0.1f;

        while (timer < 1f)
        {
            timer += Time.deltaTime / speed;
            transform.localScale = Vector3.Lerp(originalScale, hurtScale, timer);
            yield return null;
        }

        timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime / speed;
            transform.localScale = Vector3.Lerp(hurtScale, originalScale, timer);
            yield return null;
        }

        transform.localScale = originalScale;
    }
    void HandleWalkAnimation()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            float timer = Time.time * walkBobSpeed;

            float bobValue = Mathf.Sin(timer) * walkBobAmount;

            Vector3 targetScale = new Vector3(
                originalScale.x * (1 - bobValue),
                originalScale.y * (1 + bobValue),
                originalScale.z * (1 - bobValue)
            );
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);

            float swayValue = Mathf.Cos(timer) * walkSwayAmount;

            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, swayValue);
            transform.rotation = targetRotation;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 5f);

            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
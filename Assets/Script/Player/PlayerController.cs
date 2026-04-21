using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : Player
{
    [Header("Movement Settings")]
    public bool canControl = true;
    public float moveSpeed = 2f;
    public float rotateSpeed = 5f;

    [Header("Combat Settings")]
    public float attackRange = 0.25f;
    public float attackRadius = 0.25f;
    public float attackCooldown = 0.5f;

    [Header("Inventory Settings")]
    public CanvasGroup inventoryUI;
    private bool isInventoryVisible = false;


    [Header("Audio")]
    public SoundProfile attackSounds;
    public SoundProfile hurtSounds;
    public SoundProfile footstepSounds;  
    public float stepInterval = 2f;
    private float distanceTraveled;

    [Header("Animation")]
    [SerializeField] private bool isWalking = false;
    [SerializeField] private Animator animator;
    
    [Header("Effect")]
    [SerializeField] private GameObject smokeTrail;
    [SerializeField] private GameObject attackEffect;
    [SerializeField] private GameObject healEffect;
    [SerializeField] private GameObject buffEffect;

    private float lastAttackTime;
    private Rigidbody rb;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction attackAction;

    

    protected override void Start()
    {
        base.Start();

        //Animation
        isWalking = true;
        smokeTrail.SetActive(false);


        rb = GetComponent<Rigidbody>();

        var map = InputSystem.actions.FindActionMap("Player");
        if (map == null) map = InputSystem.actions.FindActionMap("Gameplay");

        if (moveAction == null) moveAction = InputSystem.actions.FindAction("Move");
        if (lookAction == null) lookAction = InputSystem.actions.FindAction("Look");
        if (attackAction == null) attackAction = InputSystem.actions.FindAction("Attack");

        ApplyUpgradeStats();
    }

    void Update()
    {
        if (!canControl)
        {
            if (animator != null) animator.SetBool("isWalking", false);
            return;
        }

        HandleRotation();
        HandleAttack();
        HandleFootsteps();

        if (Input.GetKeyDown(KeyCode.B))
        {
            //LockCursor();
            //ShowInventoryUI();
            GameManager.Instance.TriggerEndGame();
            Die();
        }
    }

    void FixedUpdate()
    {
        if (!canControl) return;

        HandleMovement();
        HandleAnimation();
    }

    private bool isCursorLocked = true;

    bool isDead = false;
    protected override void Die()
    {
        base.Die();
        isDead = true;

        if (currentHP <= 0)
        {
            if (GameManager.Instance != null)
            {
                gameObject.SetActive(false);
                GameManager.isWin = false;
                GameManager.Instance.TriggerEndGame();

                
            }

        }
    }

    void LockCursor()
    {
        isCursorLocked = !isCursorLocked;

        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetControl(bool state)
    {
        canControl = state;

        if (!state)
        {
            if (rb != null)
            {

                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }

            if (animator != null) animator.SetBool("isWalking", false);
        }
    }

    void HandleRotation()
    {
        if (!isCursorLocked) return;

        if (lookAction != null)
        {
            float mouseX = lookAction.ReadValue<Vector2>().x;
            transform.Rotate(Vector3.up * mouseX * rotateSpeed * Time.deltaTime);
        }
    }

    void HandleMovement()
    {
        if (moveAction != null)
        {
            Vector2 inputVector = moveAction.ReadValue<Vector2>();

            Vector3 moveDir = (transform.forward * inputVector.y + transform.right * inputVector.x).normalized;

            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        }
    }

    void HandleFootsteps()
    {
        Vector2 inputVector = Vector2.zero;
        if (moveAction != null)
        {
            inputVector = moveAction.ReadValue<Vector2>();
        }

        if (inputVector.magnitude > 0.1f && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            distanceTraveled += moveSpeed * Time.deltaTime;

            if (distanceTraveled >= stepInterval)
            {
                PlayFootstepSound();
                distanceTraveled -= stepInterval;
            }
        }
        else
        {
            distanceTraveled = stepInterval * 0.9f;
        }
    }

    void PlayFootstepSound()
    {
        SoundManager.Instance.PlayRandomSFX(footstepSounds);
    }

    void HandleAnimation()
    {
        if (moveAction == null) return;

        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        bool isMoving = inputVector.magnitude > 0.1f;

        if (isMoving)
        {
            isWalking = true;
            animator.SetBool("isWalking", isWalking);
            smokeTrail.SetActive(isWalking);
        }
        else
        {
            isWalking = false;
            animator.SetBool("isWalking", isWalking);
            smokeTrail.SetActive(isWalking);
        }
    }

    void HandleAttack()
    {
        if (attackAction != null && attackAction.WasPressedThisFrame() && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        Debug.Log("Player Attacked!");
        Vector3 hitPoint = transform.position + (transform.forward * attackRange);
        
        Collider[] hitColliders = Physics.OverlapSphere(hitPoint, attackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        SoundManager.Instance.PlayRandomSFX(attackSounds);
        PlayEffect(attackEffect, 0.15f , 0.1f);

    }

    private void PlayEffect(GameObject effect, float effectY, float effectX)
    {
        Vector3 effectPoint = transform.position + (transform.forward * effectX) + (transform.up * effectY);

        if (attackEffect != null)
        {
            GameObject effectInstance = Instantiate(effect, effectPoint, transform.rotation,transform);
            var ps = effectInstance.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                float lifetime = main.duration + main.startLifetime.constantMax;
                Destroy(effectInstance, lifetime + 0.1f);
            }
            else
            {
                Destroy(effectInstance, 2f);
            }
        }
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayRandomSFX(hurtSounds);
        }

    }

    public override void Heal(int amount)
    {
        base.Heal(amount);
        PlayEffect(healEffect, 0.1f, -0.2f);
    }

    public override void Buff(int amount)
    {
        base.Buff(amount);
        PlayEffect(buffEffect, 0.1f, -0.1f);
    }

    private void ShowInventoryUI()
    {
        if(inventoryUI == null) return;

        isInventoryVisible = !isInventoryVisible;
        if (isInventoryVisible)
        {
            inventoryUI.alpha = 1;
        }
        else
        {
            inventoryUI.alpha = 0;
        }
    }

    private void ApplyUpgradeStats()
    {
        switch (InventoryManager.Instance.healthLv)
        {
            case 0: maxHP = 4; break;
            case 1: maxHP = 5; break;
            case 2: maxHP = 6; break;
            case 3: maxHP = 8; break;
        }

        switch (InventoryManager.Instance.speedLv) 
        { 
            case 0 : moveSpeed = 2; break;
            case 1 : moveSpeed = 2.5f; break;
            case 2 : moveSpeed = 3; break;
            case 3 : moveSpeed = 4; break;
        }

        switch (InventoryManager.Instance.damageLv)
        {
            case 0: damage = 1; break;
            case 1: damage = 2; break;
            case 2: damage = 3; break;
            case 3: damage = 4; break;
        }

        switch (InventoryManager.Instance.backpackLv) 
        {
            case 0: InventoryManager.Instance.inventroySize = 12; break;
            case 1: InventoryManager.Instance.inventroySize = 14; break;
            case 2: InventoryManager.Instance.inventroySize = 16; break;
            case 3: InventoryManager.Instance.inventroySize = 20; break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 hitPoint = transform.position + (transform.forward * attackRange);
        Gizmos.DrawWireSphere(hitPoint, attackRadius);
    }

}
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody rb;
    

    [Header("Mouse")]
    [SerializeField] bool isCursorLocked = true;
    [SerializeField] private float rotateSpeed = 100f;
    

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private float attackDamage = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    private float lastAttackTime;

    //input action
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction attackAction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (lookAction == null) lookAction = InputSystem.actions.FindAction("Look");
        if (moveAction == null) moveAction = InputSystem.actions.FindAction("Move");
        if (attackAction == null) attackAction = InputSystem.actions.FindAction("Attack");
    }

    private void Update()
    {
        HandleRotation();
        HandleAttack();
    }

    private void FixedUpdate()
    {
        HandleMove();
    }

    private void HandleMove()
    {
        if (moveAction != null)
        {
            Vector2 inputVector = moveAction.ReadValue<Vector2>();

            Vector3 moveDir = (transform.forward * inputVector.y + transform.right * inputVector.x).normalized;

            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        }

    }

    private void HandleRotation()
    {
        if (!isCursorLocked) return;

        if (lookAction != null)
        {
            float mouseX = lookAction.ReadValue<Vector2>().x;
            transform.Rotate(Vector3.up * mouseX * rotateSpeed * Time.deltaTime);
        }
    }

    #region Attack
    private void HandleAttack()
    {
        if (attackAction != null && attackAction.WasPressedThisFrame() && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        Debug.Log("Player Attacked!");
        Vector3 hitPoint = transform.position + (transform.forward * attackRange);

        Collider[] hitColliders = Physics.OverlapSphere(hitPoint, attackRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }

    }

    private void OnDrawGizmos()
    {
        Vector3 hitPoint = transform.position + (transform.forward * attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint, attackRadius);
    }
    #endregion
}

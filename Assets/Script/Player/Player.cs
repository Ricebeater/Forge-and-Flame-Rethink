using Unity.VisualScripting;
using UnityEngine;

public class Player : Character
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Die()
    {
        base.Die();
        
        var movement = GetComponent<PlayerController>();
        if (movement != null) movement.enabled = false;
        
        var collider = GetComponent<CapsuleCollider>();
        if (collider != null) collider.enabled = false;

        var visual = GetComponentInChildren<MeshRenderer>();
        if (visual != null) visual.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

    }

    public virtual void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;


    }

    public virtual void Buff(int amount)
    {
        damage += amount;


    }
}
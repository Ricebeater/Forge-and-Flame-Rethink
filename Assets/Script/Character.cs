using System.Collections;
using UnityEngine;

public class Character : Identity
{
    [Header("Character Stats")]
    public int maxHP = 100;
    public int currentHP;
    public int damage = 10;

    [Header("Hitted Animation")]
    private Renderer objectRenderer;
    [SerializeField] private float flashDuration;
    [SerializeField] private int flashCount;
    [SerializeField] private GameObject deadEffect;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        currentHP = maxHP;

        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (currentHP <= 0) { return; }
        
        currentHP -= damageAmount;
        FlashEffect();
        Debug.Log(entityName + " Hit! RemainHp: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        PlayDeadEffect();
        Debug.Log(entityName + " Died");
    }

    public virtual void FlashEffect()
    {
        if (objectRenderer != null && gameObject.activeInHierarchy) {
            StopAllCoroutines();
            StartCoroutine(FlashRoutine());
        }
    }

    public virtual void PlayDeadEffect()
    {
        if (deadEffect != null)
        {
            GameObject deadVFX = GameObject.Instantiate(deadEffect, transform.position, transform.rotation);
        }
    }

    public virtual IEnumerator FlashRoutine()
    {
        if (objectRenderer == null) yield return null;

        float blickInterval = flashDuration / (flashCount * 2);

        for(int i = 0; i < flashCount; i++)
        {
            objectRenderer.material.SetInt("_Flash", 1);
            yield return new WaitForSeconds(blickInterval);

            objectRenderer.material.SetInt("_Flash", 0);
            yield return new WaitForSeconds(blickInterval);
        }

        objectRenderer.material.SetInt("_Flash", 0);

    }
}
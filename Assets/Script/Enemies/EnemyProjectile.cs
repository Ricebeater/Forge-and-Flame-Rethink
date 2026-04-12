using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float speed;
    private int damage;
    private Vector3 targetPosition;

    public void Setup(Vector3 targetDir, int damageAmount, float newSpeed)
    {
        this.damage = damageAmount;
        this.speed = newSpeed;

        transform.forward = targetDir;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Character target = other.GetComponent<Character>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Projectile"))
        {
            Destroy(gameObject);
        }
    }
}
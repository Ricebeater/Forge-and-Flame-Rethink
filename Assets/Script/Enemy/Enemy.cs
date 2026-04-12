using UnityEngine;

public class Enemy : MonoBehaviour
{
    public void TakeDamage(float damage)
    {
        Debug.Log("Enemy Hitted " + damage + " damage");
    }
}

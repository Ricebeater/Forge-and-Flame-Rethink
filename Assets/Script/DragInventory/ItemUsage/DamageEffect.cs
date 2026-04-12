using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Effect", menuName = "Grid/Inventory/Effet/Damage")]
public class DamageEffect : ItemEffect
{
    public int damageAmount = 10;
    public override bool ExecuteEffect(GameObject target)
    {
        Player player = target.GetComponent<Player>();
        if (player != null)
        {
            player.Buff(damageAmount);
            return true;
        }
        return false;
    }
}

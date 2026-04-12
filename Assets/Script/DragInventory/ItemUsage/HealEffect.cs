using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Effect", menuName =  "Grid/Inventory/Effet/Heal")]
public class HealEffect : ItemEffect
{
    public int healAmount = 20;

    public override bool ExecuteEffect(GameObject user)
    {
        Player player = user.GetComponent<Player>();
        if (player != null)
        {
            player.Heal(healAmount);
            return true;
        }
        return false;
    }



}

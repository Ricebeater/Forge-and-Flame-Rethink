using UnityEngine;

[CreateAssetMenu(fileName = "CustomerOrder Data", menuName = "Customer/CustomerOrder Data")]
public class CustomerOrderSO : ScriptableObject
{
    [Header("info")]
    public string customerName;
    public Sprite potrait;
    [TextArea] 
    public string orderDescription;

    [Header("Order")]
    public WeaponType wantedWeapon;
    public WeaponElement wantedElement;
}

public enum WeaponType
{
    Sword,
    Mace,
    Rapier
}
public enum WeaponElement
{
    Fire,
    Water,
    Plant
}

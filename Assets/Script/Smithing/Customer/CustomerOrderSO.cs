using UnityEngine;

[CreateAssetMenu(fileName = "CustomerOrder Data", menuName = "Customer/CustomerOrder Data")]
public class CustomerOrderSO : ScriptableObject
{
    [Header("info")]
    public string customerName;
    public Sprite potrait;

    [Header("Dialogs")]
    [TextArea] 
    public string orderDescription;
    [TextArea]
    public string goodFinish;
    [TextArea]
    public string okayFinish;
    [TextArea]
    public string badFinish;

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

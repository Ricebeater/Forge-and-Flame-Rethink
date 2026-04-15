using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }
    public CraftingStep CurrentStep { get; private set; } = CraftingStep.Idle;

    [Header("Current Order")]
    public CustomerOrderSO currentCustomerOrder;

    [Header("Player Selection")]
    public WeaponType selectedWeapon;
    public WeaponElement selectedWeaponElement;
    public OreType selectedOreType;

    private bool hasSelectedWeapon = false;
    private bool hasSelectedWeaponElement = false;
    private bool hasSelectedOreType = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectedWeaponType(int typeIndex)
    {
        selectedWeapon = (WeaponType)typeIndex;
        hasSelectedWeapon = true;
        Debug.Log($"Selected Weapon Type: {typeIndex}");
    }

    public void SelectedWeaponElement(int elementIndex)
    {
        selectedWeaponElement = (WeaponElement)elementIndex;
        hasSelectedWeaponElement = true;
        Debug.Log($"Selected Weapon Element: {elementIndex}");
    }

    public void SelectedOreType(int oreTypeIndex)
    {
        selectedOreType = (OreType)oreTypeIndex;
        hasSelectedOreType = true;
        Debug.Log($"Selected Ore Type: {oreTypeIndex}");
    }

    public void CraftWeapon()
    {
        if (!hasSelectedWeapon || !hasSelectedWeaponElement || !hasSelectedOreType)
        {
            Debug.Log("Please make all selections before crafting.");
            return;
        }
        CraftedWeapon craftedWeapon = new CraftedWeapon(selectedWeapon, selectedWeaponElement);
        
        EvaluateCraft(craftedWeapon);

        hasSelectedOreType = false;
        hasSelectedWeapon = false;
        hasSelectedWeaponElement = false;
    }

    private void EvaluateCraft(CraftedWeapon craftedWeapon)
    {
        if (currentCustomerOrder == null)
        {
            Debug.Log("No current customer order to evaluate.");
            return;
        }

        bool weaponMatch = craftedWeapon.weaponType == currentCustomerOrder.wantedWeapon;
        bool elementMatch = craftedWeapon.weaponElement == currentCustomerOrder.wantedElement;

        if (weaponMatch && elementMatch)
        {
            Debug.Log("(1) Crafting successful! The customer is satisfied.");
        }
        else if (weaponMatch || elementMatch)
        {
            Debug.Log("(0.5) Crafting partially successful. The customer is somewhat satisfied.");
        }
        else
        {
            Debug.Log("(0) Crafting failed. The customer is dissatisfied.");
        }
    }

    
}

public struct CraftedWeapon
    {
        public WeaponType weaponType;
        public WeaponElement weaponElement;

        public CraftedWeapon(WeaponType type, WeaponElement element)
        {
            weaponType = type;
            weaponElement = element;
        }
    }

public enum CraftingStep
{
    Idle,
    TakeOrder,
    Smelting,
    Forging,
    Quenching,
    Deliver
}

public enum OreType
{
    Iron,
    Silver,
    Gold
}
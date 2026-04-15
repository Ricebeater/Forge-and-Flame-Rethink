using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Current Order")]
    public CustomerOrderSO currentCustomerOrder;

    [Header("Player Selection")]
    public WeaponType selectedWeapon;
    public WeaponElement selectedWeaponElement;
    public OreType selectedOreType;

    [Header("UI element")]
    public CanvasGroup weaponSelectCanvas;

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

    private void Start()
    {
        weaponSelectCanvas.alpha = 0f;
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
            //Scoring and reward logic here
        }
        else if (weaponMatch || elementMatch)
        {
            Debug.Log("(0.5) Crafting partially successful. The customer is somewhat satisfied.");
            //Scoring and reward logic here
        }
        else
        {
            Debug.Log("(0) Crafting failed. The customer is dissatisfied.");
            //Scoring and reward logic here
        }
    }

    public void ShowWeaponSelectMenu()
    {
        if(weaponSelectCanvas != null)
        {
            weaponSelectCanvas.alpha = 1f;
            weaponSelectCanvas.blocksRaycasts = true;
            weaponSelectCanvas.interactable = true;
        }
    }

    public void HideWeaponSelectMenu()
    {
        if(weaponSelectCanvas != null)
        {
            weaponSelectCanvas.alpha = 0f;
            weaponSelectCanvas.blocksRaycasts = false;
            weaponSelectCanvas.interactable = false;
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


public enum OreType
{
    Iron,
    Silver,
    Gold
}
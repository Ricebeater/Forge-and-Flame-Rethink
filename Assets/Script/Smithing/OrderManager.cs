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

    [Header("Supplies Data")]
    public ItemData redGemData;
    public ItemData blueGemData;
    public ItemData enchantedWoodData;
    public ItemData ironData;
    public ItemData silverData;
    public ItemData goldData;

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

        hasSelectedOreType = false;
        hasSelectedWeapon = false;
        hasSelectedWeaponElement = false;
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

    public bool IsReadyToCraft()
    {
        return hasSelectedWeapon && hasSelectedWeaponElement && hasSelectedOreType;
    }

    public WeaponElement GetSelectedWeaponElement()
    {
        return selectedWeaponElement;
    }

    public OreType GetSelectedOreType()
    {
        return selectedOreType;
    }

    public void CraftWeapon()
    {
        if (currentCustomerOrder == null) return;

        ItemData requiredOre = GetSelectedOreData();
        ItemData requiredElement = GetSelectedElementData();

        if (InventoryManager.Instance == null) return;

        int oreCount = InventoryManager.Instance.GetPouchAmount(requiredOre);
        int elementCount = InventoryManager.Instance.GetPouchAmount(requiredElement);

        if (oreCount >= 1 && elementCount >= 1)
        {
            InventoryManager.Instance.RemoveFromPouch(requiredOre, 1);
            InventoryManager.Instance.RemoveFromPouch(requiredElement, 1);
            Debug.Log($"Removed 1 {requiredOre.itemName} and 1 {requiredElement.itemName}");

            PlayerInventory inventoryUI = FindAnyObjectByType<PlayerInventory>();
            if (inventoryUI != null) inventoryUI.UpdateSupplyUI();

            CraftedWeapon craftedWeapon = new CraftedWeapon(selectedWeapon, selectedWeaponElement);
            EvaluateCraft(craftedWeapon);

            hasSelectedOreType = false;
            hasSelectedWeapon = false;
            hasSelectedWeaponElement = false;

            HideWeaponSelectMenu();
            SmithingManager.Instance.StartSmithing();
        }
        else
        {
            Debug.Log("Not enough resources to craft this weapon!");
        }
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

    private ItemData GetSelectedOreData()
    {
        switch (selectedOreType)
        {
            case OreType.Iron: return ironData;
            case OreType.Silver: return silverData;
            case OreType.Gold: return goldData;
            default: return null;
        }
    }

    private ItemData GetSelectedElementData()
    {
        switch (selectedWeaponElement)
        {
            case WeaponElement.Fire: return redGemData;
            case WeaponElement.Water: return blueGemData;
            case WeaponElement.Plant: return enchantedWoodData;
            default: return null;
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
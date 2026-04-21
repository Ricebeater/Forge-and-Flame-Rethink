using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Global Inventory Data")]

    private List<ItemData> savedGridItems = new List<ItemData>();
    public Dictionary<ItemData, int> supplyPouch = new Dictionary<ItemData, int>();

    public List<ItemData> huntBag = new List<ItemData>();

    [Header("Player Upgrade")]
    public int healthLv   = 0;
    public int speedLv    = 0;
    public int damageLv   = 0;
    public int backpackLv = 0;

    public int inventroySize = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddToPouch(ItemData item, int amount = 1)
    {
        if (supplyPouch.ContainsKey(item))
        {
            supplyPouch[item] += amount;
        }
        else
        {
            supplyPouch.Add(item, amount);
        }
    }

    public bool RemoveFromPouch(ItemData item, int amount = 1)
    {
        if (supplyPouch.ContainsKey(item) && supplyPouch[item] >= amount)
        {
            supplyPouch[item] -= amount;
            return true;
        }
        return false;
    }

    public int GetPouchAmount(ItemData item)
    {
        if (item == null) return 0;
        return supplyPouch.TryGetValue(item, out int amount) ? amount : 0;
    }

    public void EmptyHuntBagIntoPouch()
    {
        foreach (ItemData material in huntBag)
        {
            AddToPouch(material, 1);
        }
        huntBag.Clear();
        Debug.Log($"Emptied Hunt Bag! Pouch now has {supplyPouch.Count} unique material types.");
    }


    public void SaveGridInventory(PlayerInventory inventory)
    {
        savedGridItems.Clear();
        for (int x = 0; x < inventory.gridWidth; x++)
        {
            for (int y = 0; y < inventory.gridHeight; y++)
            {
                InventoryItem item = inventory.slots[x, y];
                if (item != null && item.gridX == x && item.gridY == y)
                {
                    savedGridItems.Add(item.itemData);
                    item.transform.SetParent(null);
                    DontDestroyOnLoad(item.gameObject);
                }
            }
        }
        Debug.Log($"Backpack saved with {savedGridItems.Count} grid items.");
    }

    public void LoadGridInventory(PlayerInventory inventory)
    {
        foreach (ItemData data in savedGridItems)
        {
            InventoryItem[] allItems = FindObjectsByType<InventoryItem>(FindObjectsSortMode.None);
            foreach (InventoryItem item in allItems)
            {
                if (item.itemData == data)
                {
                    inventory.AutoPlaceItem(item);
                    break;
                }
            }
        }
    }
    public void ResetData()
    {
        supplyPouch.Clear();
        huntBag.Clear();
        savedGridItems.Clear();

        healthLv = 0;
        speedLv = 0;
        damageLv = 0;
        backpackLv = 0;

        Debug.Log("Inventory and Upgrades have been wiped clean.");
    }
}
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private List<ItemData> savedItems = new List<ItemData>();

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

    public void SavePlayerInventory(PlayerInventory inventory)
    {
        savedItems.Clear();

        for (int x = 0; x < inventory.gridWidth; x++)
        {
            for (int y = 0; y < inventory.gridHeight; y++)
            {
                InventoryItem item = inventory.slots[x, y];

                if (item != null && item.gridX == x && item.gridY == y)
                {
                    savedItems.Add(item.itemData);
                    item.transform.SetParent(null);
                    DontDestroyOnLoad(item.gameObject);
                }
            }
        }

        Debug.Log("Player inventory saved with " + savedItems.Count + " items.");
    }

    public void LoadPlayerInventory(PlayerInventory inventory)
    {
        inventory.LoadSuppliesFromList(savedItems);

        foreach (ItemData data in savedItems)
        {
            InventoryItem[] allItems = FindObjectsByType<InventoryItem>(FindObjectsSortMode.None);
            foreach (InventoryItem item in allItems)
            {
                if (item.itemData == data)
                {
                    inventory.AutoPlaceItem(item);
                    Debug.Log("Loaded item: " + item.itemData.itemName);
                    break;
                }
            }
        }
    }

}
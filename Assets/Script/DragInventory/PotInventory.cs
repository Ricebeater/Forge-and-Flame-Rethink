using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotInventory : InventoryBase
{
    [Header("Crafting Setting")]
    public List<CraftingRecipe> recipes;
    public Button craftButton;
    public GameObject itemPrefab;


    protected override void Awake()
    {
        base.Awake();

        if (craftButton != null)
        {
            craftButton.onClick.AddListener(TryCraft);
        }

        slots = new InventoryItem[gridWidth, gridHeight];
    }

    private void Start()
    {
        cellSize = GlobalSetteing.GridSize;
        rectTransform.sizeDelta = new Vector2(gridWidth * cellSize, gridHeight * cellSize);
        //CreateSlot();
    }

    private void TryCraft()
    {
        Debug.Log("Attempting to craft item...");
        List<ItemData> currentIngredients = GetAllItemInPot();
        foreach (CraftingRecipe recipe in recipes)
        {
            if (CanCraftItem(currentIngredients, recipe.requiredIngredients))
            {
                // Craft the item
                CraftItem(recipe);
                return;
            }
        }
        
    }

    private void CraftItem(CraftingRecipe recipe)
    {
        ClearInventory();

        AddCraftedItem(recipe.resultItem);
    }

    private void AddCraftedItem(ItemData resultData)
    {
        GameObject newItemObj = Instantiate(itemPrefab, transform.parent);

        InventoryItem newItem = newItemObj.GetComponent<InventoryItem>();
        DragableItem dragable = newItemObj.GetComponent<DragableItem>();

        newItem.itemData = resultData;
        newItem.name = resultData.itemName;
        newItem.grid = this;
        
        if(dragable != null)
        {
            dragable.currentGrid = this;
        }

        if(CanPlaceItem(newItem, 0, 0))
        {
            PlaceItem(newItem, 0, 0);
        }
        else
        {
            Debug.Log("Not enough space to place crafted item.");
            Destroy(newItemObj);
        }
    }

    private bool CanCraftItem(List<ItemData> itemInPot, List<ItemData> requiredItem)
    {
        if(itemInPot.Count != requiredItem.Count)
        {
            return false;
        }

        List<ItemData> tempItemInPot = new List<ItemData>(itemInPot);
        foreach (ItemData ingredient in requiredItem)
        {
            if (tempItemInPot.Contains(ingredient))
            {
                tempItemInPot.Remove(ingredient);
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private List<ItemData> GetAllItemInPot()
    {
        List<ItemData> items = new List<ItemData>();
        List<InventoryItem> processedItems = new List<InventoryItem>();

        for(int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                InventoryItem item = slots[x, y];
                if (item != null && !processedItems.Contains(item))
                {
                    items.Add(item.itemData);
                    processedItems.Add(item);
                }
            }
        }
        return items;
    }
}

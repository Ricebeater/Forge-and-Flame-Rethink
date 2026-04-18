using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] private InventoryBase grid;
    [SerializeField] private RectTransform inventory;
    [SerializeField] private GameObject itemPrefab;


    private void OnTriggerEnter(Collider other)
    {
        WorldItem worldItem = other.GetComponent<WorldItem>();

        if (worldItem == null)
            return;

        if (worldItem.itemData.itemType == ItemData.ItemType.material)
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.huntBag.Add(worldItem.itemData);
                Debug.Log($"Collected {worldItem.itemData.itemName} into Hunt Bag.");
                Destroy(other.gameObject);
            }
        }

        if (AddItemToInventory(worldItem.itemData))
        {
            Destroy(other.gameObject);
        }
    }

    private bool AddItemToInventory(ItemData newItemData)
    {
        GameObject itemUI = Instantiate(itemPrefab, inventory.transform);
        
        InventoryItem item = itemUI.GetComponent<InventoryItem>();
        DragableItem dragable = itemUI.GetComponent<DragableItem>();

        item.itemData = newItemData;
        item.name = newItemData.itemName;

        item.rectTransform = item.GetComponent<RectTransform>();

        item.grid = grid;
        dragable.currentGrid = grid;

        for (int x = 0; x < grid.gridWidth; x++)
        {
            for (int y = 0; y < grid.gridHeight; y++)
            {
                if (grid.CanPlaceItem(item, x, y))
                {
                    grid.PlaceItem(item, x, y);
                    item.rectTransform.anchoredPosition = grid.GetItemPositionInUI(x, y);
                    return true;
                }
            }
        }

        Debug.Log("No space to add the item in the inventory.");
        Destroy(itemUI);
        return false;
    }

}

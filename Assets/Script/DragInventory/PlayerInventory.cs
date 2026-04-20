using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : InventoryBase
{
    [Header("Starting Items (Debug)")]
    [SerializeField] private List<ItemData> startingItems;

    [Header("Supplies UI Text")]
    [SerializeField] private TextMeshProUGUI redGemCountText;
    [SerializeField] private TextMeshProUGUI blueGemCountText;
    [SerializeField] private TextMeshProUGUI enchantedWoodCountText;
    [SerializeField] private TextMeshProUGUI ironCountText;
    [SerializeField] private TextMeshProUGUI silverCountText;
    [SerializeField] private TextMeshProUGUI goldCountText;

    private void Update()
    {

    }

    protected override void Awake()
    {
        base.Awake();
        
        rectTransform = GetComponent<RectTransform>();
    }

    private IEnumerator Start()
    {
        yield return null;
        
        gridWidth = InventoryManager.Instance.inventroySize;

        slots = new InventoryItem[gridWidth, gridHeight];

        cellSize = GlobalSetteing.GridSize;
        rectTransform.sizeDelta = new Vector2(gridWidth * cellSize, gridHeight * cellSize);
           
        if (OrderManager.Instance != null)
        {
            InventoryManager.Instance.EmptyHuntBagIntoPouch();
        }
        
        addTestItem(startingItems);
        UpdateSupplyUI();
    }

    public void UpdateSupplyUI()
    {

        if (OrderManager.Instance != null && InventoryManager.Instance != null)
        {
            if (redGemCountText != null)        redGemCountText.text        = InventoryManager.Instance.GetPouchAmount(OrderManager.Instance.redGemData).ToString();
            if (blueGemCountText != null)       blueGemCountText.text       = InventoryManager.Instance.GetPouchAmount(OrderManager.Instance.blueGemData).ToString();
            if (enchantedWoodCountText != null) enchantedWoodCountText.text = InventoryManager.Instance.GetPouchAmount(OrderManager.Instance.enchantedWoodData).ToString();
            if (ironCountText != null)          ironCountText.text          = InventoryManager.Instance.GetPouchAmount(OrderManager.Instance.ironData).ToString();
            if (silverCountText != null)        silverCountText.text        = InventoryManager.Instance.GetPouchAmount(OrderManager.Instance.silverData).ToString();
            if (goldCountText != null)          goldCountText.text          = InventoryManager.Instance.GetPouchAmount(OrderManager.Instance.goldData).ToString();
        }
    }

    #region Debug
    private void addTestItem(List<ItemData> testItemList)
    {
        if (testItemList == null || InventoryManager.Instance == null) return;

        foreach (ItemData item in testItemList)
        {
            if (item != null && item.itemType == ItemData.ItemType.material)
            {
                InventoryManager.Instance.AddToPouch(item, 1);
            }
        }
    }
    #endregion
}
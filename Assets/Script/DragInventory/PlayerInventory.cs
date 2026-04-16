using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInventory : InventoryBase
{
    [SerializeField] public List<ItemData> startingItems;

    public Dictionary<ItemData, int> supplyPouch = new Dictionary<ItemData, int>();

    [Header("Supplies UI")]
    [SerializeField] private TextMeshProUGUI redGemCountText;
    [SerializeField] private TextMeshProUGUI blueGemCountText;
    [SerializeField] private TextMeshProUGUI enchantedWoodCountText;
    [SerializeField] private TextMeshProUGUI ironCountText;
    [SerializeField] private TextMeshProUGUI silverCountText;
    [SerializeField] private TextMeshProUGUI goldCountText;

    [Header("Supplies Data")]
    [SerializeField] private ItemData redGemData;
    [SerializeField] private ItemData blueGemData;
    [SerializeField] private ItemData enchantedWoodData;
    [SerializeField] private ItemData ironData;
    [SerializeField] private ItemData silverData;
    [SerializeField] private ItemData goldData;

    private void Update()
    {
        if(Keyboard.current.cKey.wasPressedThisFrame)
        {
            InventoryManager.Instance.SavePlayerInventory(this);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        slots = new InventoryItem[gridWidth, gridHeight];
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        cellSize = GlobalSetteing.GridSize;
        rectTransform.sizeDelta = new Vector2(gridWidth * cellSize, gridHeight * cellSize);
        //CreateSlot();

        if(InventoryManager.Instance != null)
        {
            InventoryManager.Instance.LoadPlayerInventory(this);
        }

        addTestItem(startingItems);
    }

    public void LoadSuppliesFromList(List<ItemData> savedList)
    {
        supplyPouch.Clear();

        foreach (ItemData item in savedList)
        {
            if(item.itemType == ItemData.ItemType.material)
            {
                AddSupply(item, 1);
            }
        }

        Debug.Log($"Loaded supplies! Pouch now has {supplyPouch.Count} unique types of materials.");
    }

    public void AddSupply(ItemData item, int amount = 1)
    {
        if (supplyPouch.ContainsKey(item))
        {
            supplyPouch[item] += amount;
        }
        else
        {
            supplyPouch.Add(item, amount);
        }
        UpdateSupplyUI();

    }

    public bool RemoveSupply(ItemData item, int amount = 1)
    {
        if (supplyPouch.ContainsKey(item) && supplyPouch[item] >= amount)
        {
            supplyPouch[item] -= amount;
            
            return true;
        }

        Debug.Log("not enough supply");
        return false;
    }

    public void UpdateSupplyUI()
    {
        redGemCountText.text        = GetAmount(redGemData).ToString();
        blueGemCountText.text       = GetAmount(blueGemData).ToString();
        enchantedWoodCountText.text = GetAmount(enchantedWoodData).ToString();
        ironCountText.text          = GetAmount(ironData).ToString();
        silverCountText.text        = GetAmount(silverData).ToString();
        goldCountText.text          = GetAmount(goldData).ToString();
    }

    private int GetAmount(ItemData item)
    {
        return supplyPouch.TryGetValue(item, out int amount) ? amount : 0;
    }

    #region Debug

    private void addTestItem(List<ItemData> testItemList)
    {
        supplyPouch.Clear();

        foreach (ItemData item in testItemList)
        {
            if (item.itemType == ItemData.ItemType.material)
            {
                AddSupply(item, 1);
            }
        }
    }

    #endregion

}

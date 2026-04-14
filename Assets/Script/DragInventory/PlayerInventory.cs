using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInventory : InventoryBase
{
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
    }
}

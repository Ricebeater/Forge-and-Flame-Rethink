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
            ShowItemInInventory();
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
    }

    private void ShowItemInInventory()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (slots[x, y] != null)
                {
                    Debug.Log(slots[x, y].name);
                }
            }
        }
    }
}

using TMPro;
using UnityEngine;

public class DisplayMeatAmount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI meatAmountDisplay;
    
    private int meatAmount;
    private PlayerInventory playerInventory;
    private PotInventory potInventory;
    private ItemData data;

    private void Awake()
    {
        playerInventory = FindAnyObjectByType<PlayerInventory>();
        potInventory = FindAnyObjectByType<PotInventory>();

        if (playerInventory != null) { Debug.Log("no player inventory found"); }
        if (potInventory != null) { Debug.Log("no pot inventory found"); }
    }

    private void Update()
    {
        int meatTotal = 0;

        if (playerInventory != null)
        {
            meatTotal += CountMeat(playerInventory);
        }

        if (potInventory != null)
        {
            meatTotal += CountMeat(potInventory);
        }

        if (meatAmountDisplay != null)
        {
            meatAmountDisplay.text = meatTotal.ToString();
        }
    }

    public int CountMeat(InventoryBase inventory)
    {
        int meatCount = 0;

        for (int x = 0; x < inventory.gridWidth; x++)
        {
            for (int y = 0; y < inventory.gridHeight; y++)
            {
                InventoryItem item = inventory.slots[x, y];

                if (item != null)
                {
                    if (item.gridX == x && item.gridY == y)
                    {
                        if(item.itemData.name == "Meat")
                        {
                            meatCount++;
                        }
                    }
                }
            }
        }

        return meatCount;
    }
}

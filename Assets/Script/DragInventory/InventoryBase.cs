using UnityEngine;

public class InventoryBase : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;

    public int cellSize = 32;

    public bool isDebugging = false;

    public InventoryItem[,] slots;

    public RectTransform rectTransform;

    protected virtual void Awake()
    {
        slots = new InventoryItem[gridWidth, gridHeight];
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            FindAndEatMeat(this);
        }
    }

    public bool GetMousePosInGrid(Vector2 screenPos, out int x, out int y)
    {
        x = 0; y = 0;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPos, null, out localPoint))
        {
            return false;
        }

        Vector2 size = rectTransform.rect.size;

        float originX = localPoint.x + (size.x * rectTransform.pivot.x);
        float originY = (size.y * (1f - rectTransform.pivot.y)) - localPoint.y;

        x = Mathf.FloorToInt(originX / cellSize);
        y = Mathf.FloorToInt(originY / cellSize);


        return true;
    }

    public bool CanPlaceItem(InventoryItem item, int pointingX, int pointingY)
    {
        int itemWidth = item.Width;
        int itemHeight = item.Height;

        if (pointingX < 0 || pointingY < 0) { return false; }
        if (pointingX + itemWidth > gridWidth || pointingY + itemHeight > gridHeight) { return false; }

        for (int x = pointingX; x < pointingX + itemWidth; x++)
        {
            for (int y = pointingY; y < pointingY + itemHeight; y++)
            {
                if (slots[x, y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }


    public void PlaceItem(InventoryItem item, int pointingX, int pointingY)
    {
        int itemWidth = item.Width;
        int itemHeight = item.Height;

        for (int x = pointingX; x < pointingX + itemWidth; x++)
        {
            for (int y = pointingY; y < pointingY + itemHeight; y++)
            {
                slots[x, y] = item;
            }
        }

        item.gridX = pointingX;
        item.gridY = pointingY;

        item.rectTransform.SetParent(this.rectTransform);

        item.grid = this;

        if (item.rectTransform != null)
        {
            item.rectTransform.anchoredPosition = GetItemPositionInUI(pointingX, pointingY);
        }
        SlotUpdate();
    }

    public bool AutoPlaceItem(InventoryItem item)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (CanPlaceItem(item, x, y))
                {
                    if (item.grid != null)
                    {
                        item.grid.RemoveItem(item);
                    }

                    PlaceItem(item, x, y);
                    return true;
                
                }
            }
        }
        return false;
    }

    public void FindAndEatMeat(InventoryBase inventory)
    {
        for (int x = 0; x < inventory.gridWidth; x++)
        {
            for (int y = 0; y < inventory.gridHeight; y++)
            {
                InventoryItem item = inventory.slots[x, y];

                if (item != null)
                {
                    if (item.gridX == x && item.gridY == y)
                    {
                        if (item.itemData.itemName == "Meat")
                        {
                            item.UseItem();
                            return;
                        }
                    }
                }
            }
        }

        return;
    }

    public void RemoveItem(InventoryItem item)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (slots[x, y] == item)
                {
                    slots[x, y] = null; Debug.Log("Remove item from : " + x + " " + y);
                }
            }
        }
    }

    public Vector2 GetItemPositionInUI(int x, int y)
    {
        return new Vector2(x * cellSize, -(y * cellSize));
    }

    public void ClearInventory()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (slots[x, y] != null)
                {
                    Destroy(slots[x, y].gameObject);
                }
                slots[x, y] = null;
            }
        }
    }

    //Debug
    public void SlotUpdate()
    {
        // The grid dimensions should typically be retrieved from the slots array itself
        int gridWidth = slots.GetLength(0);  // Rows/X-dimension
        int gridHeight = slots.GetLength(1); // Columns/Y-dimension

        // A StringBuilder is more efficient than repeatedly concatenating strings
        // when building large strings in a loop.
        System.Text.StringBuilder inventoryDisplay = new System.Text.StringBuilder();
        if(!isDebugging)
        {
            return;
        }
        // Loop through the dimensions to build the display row by row
        for (int x = 0; x < gridWidth; x++) // x is the Row index
        {
            string rowString = ""; // Start a new string for each row

            for (int y = 0; y < gridHeight; y++) // y is the Column index
            {
                // 1. Check the content of the slot
                InventoryItem item = slots[x, y];

                if (item != null)
                {
                    // Not null space (Occupied) => Use 'O'

                    rowString += " O";
                }
                else
                {
                    // Null space (Empty) => Use 'X'
                    rowString += " X";
                }

                // OPTIONAL: Also log the detailed info for traditional debugging
                //Debug.Log($"Slot [{x}, {y}]: Contain => {(item != null ? item.ToString() : "NULL")}");
            }

            // After processing all columns in a row, append the row string 
            // and a newline character to the main StringBuilder.
            inventoryDisplay.AppendLine(rowString);
        }

        // 2. Print the final, formatted rectangle to the Console/Log once.
        // Use Debug.LogError or Debug.LogWarning to make the rectangle stand out.
        Debug.LogWarning("--- Inventory Grid (O=Full, X=Empty) ---");
        Debug.Log(inventoryDisplay.ToString());
        Debug.Log("Slot Updated");


    }
}

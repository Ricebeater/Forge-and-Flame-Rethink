using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryBase grid;
    public ItemData itemData;
    public Image icon;

    public RectTransform rectTransform;

    public int gridX;
    public int gridY;

    [SerializeField] private bool isRotated = false;

    public int Width => isRotated ? itemData.height : itemData.width;
    public int Height => isRotated ? itemData.width : itemData.height;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        icon = GetComponent<Image>();
    }

    private void Start()
    {
        icon.sprite = itemData.icon;
        rectTransform.sizeDelta = new Vector2(itemData.width * GlobalSetteing.GridSize, itemData.height * GlobalSetteing.GridSize);
    }

    private void Update()
    {
        if(isHovered && Input.GetKeyDown(KeyCode.X))
        {
            TrashItem();
        }
    }

    private void TrashItem()
    {
        grid.RemoveItem(this);

        Destroy(gameObject);
    }

    public void RotateItem()
    {
        isRotated = !isRotated;

        rectTransform.rotation = Quaternion.Euler(0, 0, isRotated ? 90 : 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
        if (Input.GetKey(KeyCode.LeftShift) && eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Attemp Quickswap");
            QuickSwapInventory();
        }
    }

    private bool isInPlayerInventory = true;
    private void QuickSwapInventory()
    {
        InventoryBase targetGrid = null;

        isInPlayerInventory = grid is not PotInventory;

        if (isInPlayerInventory)
        {
            targetGrid = FindAnyObjectByType<PotInventory>();
        }
        else
        {
            targetGrid = FindAnyObjectByType<PlayerInventory>();
        }

        if (targetGrid != null)
        {
            targetGrid.AutoPlaceItem(this);
        }
    }

    public void UseItem()
    {
        GameObject player = GameObject.FindAnyObjectByType<Player>().gameObject;

        if (itemData.itemEffect == null) { return; }

        bool success = itemData.itemEffect.ExecuteEffect(player);

        if (success && itemData.itemType == ItemData.ItemType.consumable)
        {
            ConsumeItem();
        }

        if(success && itemData.itemType == ItemData.ItemType.equipment)
        {
            EquipItem();
        }
    }

    private void ConsumeItem()
    {
        grid.RemoveItem(this);
        Debug.Log("Item Consumed: " + itemData.itemName);
        Destroy(gameObject);
    }

    private void EquipItem()
    {
        grid.RemoveItem(this);
        Debug.Log("Item Equipped: " + itemData.itemName);
        Destroy(gameObject);
    }

    private bool isHovered = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}

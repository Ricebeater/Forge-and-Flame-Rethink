using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Grid/Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;

    [Header("Size")]
    public int _width;
    public int width
    {
        get
        {
            return _width;
        }
    }

    public int _height;
    public int height
    {
        get
        {
            return _height;
        }
    }

    [Header("Item Usage")]
    public ItemEffect itemEffect;

    [Header("Prefab & Sprite")]
    public GameObject itemPrefab;
    public Sprite icon;
    
    public enum ItemType
    {
        equipment,
        consumable,
        material
    }
}

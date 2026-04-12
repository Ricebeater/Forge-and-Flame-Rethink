using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Grid/Inventory/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<ItemData> requiredIngredients;
    public ItemData resultItem;
}

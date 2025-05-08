using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [System.Serializable]
    public struct Ingredient
    {
        public InventoryItem item;
        public int quantity;
    }

    public List<Ingredient> ingredients;
    public InventoryItem resultItem;
    public int resultQuantity = 1;
}

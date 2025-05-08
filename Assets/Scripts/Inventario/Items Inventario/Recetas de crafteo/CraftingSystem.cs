using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public List<CraftingRecipe> allRecipes;

    public bool CanCraft(CraftingRecipe recipe, Inventory inventory)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int total = inventory.getTotalAmount(ingredient.item);
            if (total < ingredient.quantity)
            {
                return false;
            }
        }
        return true;
    }

    public void Craft(CraftingRecipe recipe, Inventory inventory)
    {
        if (!CanCraft(recipe, inventory)) return;

        foreach (var ingredient in recipe.ingredients)
        {
            inventory.RemoveItem(ingredient.item, ingredient.quantity);
        }
    }
}

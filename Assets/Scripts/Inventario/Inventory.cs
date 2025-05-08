using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public InventoryItem item;
    public int quantity;
}
public class Inventory : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> items = new List<InventorySlot>();
    public int maxSlots = 20;

    public List<InventorySlot> ItemList { get{ return items; } }

    public void AddItem(InventoryItem newItem, int amount = 1)
    {
        if (newItem is AmmoItem ammoItem)
        {
            int amountToAdd = amount;

            foreach (InventorySlot slot in items)
            {
                if (slot.item == newItem && slot.quantity < ammoItem.maxAmountPerStack)
                {
                    int spaceLeft = ammoItem.maxAmountPerStack - slot.quantity;
                    int addNow = Mathf.Min(spaceLeft, amountToAdd);
                    slot.quantity += addNow;
                    amountToAdd -= addNow;

                    if (amountToAdd <= 0)
                        return;
                }
            }

            while (amountToAdd > 0 && items.Count < maxSlots)
            {
                int addNow = Mathf.Min(ammoItem.maxAmountPerStack, amountToAdd);
                items.Add(new InventorySlot { item = newItem, quantity = addNow });
                amountToAdd -= addNow;
            }
        }
        else if (isStackable(newItem))
        {
            InventorySlot existingSlot = items.Find(s => s.item == newItem);
            if (existingSlot != null)
            {
                existingSlot.quantity += amount;
            }
            else
            {
                if (items.Count >= maxSlots)
                {
                    Debug.Log("INVENTARIO LLENO");
                    return;
                }
                items.Add(new InventorySlot { item = newItem, quantity = amount });
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                if (items.Count >= maxSlots)
                {
                    Debug.Log("INVENTARIO LLENO");
                    return;
                }
                items.Add(new InventorySlot { item = newItem, quantity = 1});
            }
        }
    }

    private bool isStackable(InventoryItem item)
    {
        return item is HealingItem || item is CraftingResourceItem;
    }

    public int getTotalAmount(InventoryItem item)
    {
        int total = 0;
        foreach (var slot in items)
        {
            if (slot.item == item)
            {
                total += slot.quantity;
            }
        }
        return total;
    }

    public void RemoveItem(InventoryItem item, int amount)
    {
        for (int i = 0; i < items.Count && amount > 0; i++)
        {
            if (items[i].item == item)
            {
                int toRemove = Mathf.Min(amount, items[i].quantity);
                items[i].quantity -= toRemove;
                amount -= toRemove;

                if (items[i].quantity <= 0)
                {
                    items.RemoveAt(i--);
                }
            }
        }
    }

    public bool CanAddItem(InventoryItem item, int quantity, List<CraftingRecipe.Ingredient> simulateIngredientRemoval = null)
    {
        List<InventorySlot> tempSlots = new List<InventorySlot>();

        foreach (var s in items)
        {
            tempSlots.Add(new InventorySlot { item = s.item, quantity = s.quantity});
        }

        int tempMaxSlots = maxSlots;

        if (simulateIngredientRemoval != null)
        {
            foreach (var ingredient in simulateIngredientRemoval)
            {
                int toRemove = ingredient.quantity;
                for (int i = 0; i < tempSlots.Count && toRemove > 0; i++)
                {
                    if (tempSlots[i].item == ingredient.item)
                    {
                        int removable = Mathf.Min(toRemove, tempSlots[i].quantity);
                        tempSlots[i].quantity -= removable;
                        toRemove -= removable;

                        if (tempSlots[i].quantity <= 0)
                        {
                            tempSlots.RemoveAt(i--);
                        }
                    }
                }
            }
        }

        int amountToAdd = quantity;
        if (item is AmmoItem ammoItem)
        {
            foreach (var slot in tempSlots)
            {
                if (slot.item == item && slot.quantity < ammoItem.maxAmountPerStack)
                {
                    int add = Mathf.Min(amountToAdd, ammoItem.maxAmountPerStack - slot.quantity);
                    slot.quantity += add;
                    amountToAdd -= add;
                    if (amountToAdd <= 0)
                    {
                        return true;
                    }
                }
            }

            while (amountToAdd > 0)
            {
                if (tempSlots.Count >= tempMaxSlots)
                {
                    return false;
                }
                int add = Mathf.Min(ammoItem.maxAmountPerStack, amountToAdd);
                tempSlots.Add(new InventorySlot { item = item, quantity = add });
                amountToAdd -= add;
            }

            return true;
        } else if (isStackable(item)) 
        {
            var slot = tempSlots.Find(s => s.item == item);
            if (slot != null) return true;
            return tempSlots.Count < tempMaxSlots;
        }

        if (tempSlots.Count + amountToAdd > tempMaxSlots)
        {
            return false;
        }

        return true;
    }
}

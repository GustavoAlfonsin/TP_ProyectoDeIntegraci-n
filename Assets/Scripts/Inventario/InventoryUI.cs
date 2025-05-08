using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject slotPrefab;
    public Transform gridParent;

    private List<InventorySlotUI> inventoryList = new List<InventorySlotUI>();

    private void Start()
    {
        initializeSlots();
        RefreshInventory();
    }

    private void initializeSlots()
    {
        for (int i = 0; i < inventory.maxSlots; i++)
        {
            GameObject go = Instantiate(slotPrefab,gridParent);
            InventorySlotUI slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.clearSlot();
            inventoryList.Add(slotUI);
        }
    }

    public void RefreshInventory()
    {
        foreach (var slotUI in inventoryList) 
        {
            slotUI.clearSlot();
        }

        for (int i = 0; i < inventory.ItemList.Count && i < inventoryList.Count; i++)
        {
            var data = inventory.ItemList[i];
            inventoryList[i].setSlot(data.item._icon, data.quantity);
        }
    }
}

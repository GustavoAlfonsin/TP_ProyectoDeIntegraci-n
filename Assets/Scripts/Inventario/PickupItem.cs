using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PickupItem : MonoBehaviour
{
    public InventoryItem itemData;
    public int quantity = 1;

    private bool playerInRange = false;

    private void Start()
    {
        if (itemData == null) return;

        if (itemData is AmmoItem)
        {
            quantity = Random.Range(3,11);
        }
        else
        {
            quantity = 1;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            TryPickUp();
        }
    }

    public void TryPickUp()
    {
        if (UIController.Instance.inventory == null) return;

        int pickedAmount = UIController.Instance.inventory.AddItem(itemData, quantity);

        if (pickedAmount == quantity)
        {
            Destroy(gameObject);
        }
        else if(pickedAmount > 0)
        {
            quantity -= pickedAmount;
        }
    }
}

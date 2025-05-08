using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityTxt;

    public void setSlot(Sprite itemIcon, int quantity)
    {
        icon.sprite = itemIcon;
        icon.enabled = true;
        quantityTxt.text = quantity > 1 ? quantity.ToString() : "";
    }

    public void clearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
        quantityTxt.text = "";
    }
}

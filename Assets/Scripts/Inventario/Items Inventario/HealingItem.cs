using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Healing")]
public class HealingItem : InventoryItem
{
    [Range(0, 1f)]
    public float healPercentage;
}

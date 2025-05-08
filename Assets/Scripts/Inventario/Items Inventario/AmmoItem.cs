using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType { Pistol, Shotgun, Rifle}

[CreateAssetMenu(menuName = "Items/Ammo")]
public class AmmoItem : InventoryItem
{
    public AmmoType _AmmoType;
    public int currentAmount;
    public int maxAmountPerStack;
}

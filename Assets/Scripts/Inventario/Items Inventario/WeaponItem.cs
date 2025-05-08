using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponItem : InventoryItem
{
    public float damage;
    public float accuracy;
    public int maxAmmo;
    public AmmoType _ammoType;
    public float fireRate;
    public float reloadTime;
}

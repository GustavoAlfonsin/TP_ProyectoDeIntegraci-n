using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Ammo,
    Healing,
    CraftingResource
}

public class InventoryItem : ScriptableObject
{
    public string _itemName;
    [TextArea] public string _description;
    public Sprite _icon;
    public ItemType _type;
}

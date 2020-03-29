using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item data", menuName = "ScriptableObjects/ItwItemPreset")]
public class ItemPreset : ScriptableObject
{
    [Header("ItemPreset")]
    public new string name = "New Item";
    public string description = "This is a new Item";
    [Space]
    public Sprite sprite;
    public int stackSize = 1;
}

[System.Serializable]
public class ItwItemData
{
    [Header("ItemData")]
    public ItemPreset itemPreset;
    public int itemAmount;

    public ItwItemData(ItemPreset _itemPreset, int _itemAmount)
    {
        itemPreset = _itemPreset;
        itemAmount = _itemAmount;
    }

    public override bool Equals(object obj)
    {
        // If both itemPresets are equal, return true
        return (((ItwItemData)obj).itemPreset == itemPreset);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
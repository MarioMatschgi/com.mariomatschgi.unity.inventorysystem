using UnityEngine;

namespace MM.Systems.InventorySystem
{
    [System.Serializable]
    public class ItemData
    {
        [Header("ItemData")]
        public ItemPreset itemPreset;
        public int itemAmount;

        /// <summary>
        /// Constructor for ItemData with a preset <paramref name="_itemPreset"/> and an amount <paramref name="_itemAmount"/>
        /// </summary>
        /// <param name="_itemPreset"></param>
        /// <param name="_itemAmount"></param>
        public ItemData(ItemPreset _itemPreset, int _itemAmount)
        {
            itemPreset = _itemPreset;
            itemAmount = _itemAmount;
        }

        public override bool Equals(object obj)
        {
            if (((ItemData)obj) == null)
            {
                if (itemPreset == null)
                    return true;
                else
                    return false;
            }

            // If both itemPresets are equal, return true
            return (((ItemData)obj).itemPreset == itemPreset);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

using UnityEngine;

namespace MM.Systems.InventorySystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Item preset", menuName = "ScriptableObjects/MM InventorySystem/ItemPreset")]
    public class ItemPreset : ScriptableObject
    {
        [Header("ItemPreset")]
        public new string name = "New Item preset";
        public string description = "This is a new Item preset";
        [Space]
        public ItemType type = ItemType.Normal;
        [Space]
        public Sprite sprite;
        public int stackSize = 10;
    }
}
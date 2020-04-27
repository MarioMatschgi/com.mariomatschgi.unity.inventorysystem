using UnityEngine;

namespace MM.Systems.InventorySystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New ArmorItem preset", menuName = "ScriptableObjects/MM InventorySystem/ArmorItemPreset")]
    public class ArmorItemPreset : ItemPreset
    {
        [Header("Armor")]
        public ArmorType armorType;


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

        void Reset()
        {
            // Set default values
            type = ItemType.Armor;
            stackSize = 1;
        }

        #endregion

        #region Gameplay Methodes
        /*
         *
         *  Gameplay Methodes
         *
         */

        #endregion

        #region Helper Methodes
        /*
         *
         *  Helper Methodes
         * 
         */

        #endregion
    }

    public enum ArmorType
    {
        None,
        Helmet,
        Chestplate,
        Trousers,
        Boots,
    }
}
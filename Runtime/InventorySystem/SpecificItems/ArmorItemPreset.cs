using System.Collections;
using System.Collections.Generic;
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
    }
}
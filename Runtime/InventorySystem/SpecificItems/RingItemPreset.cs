﻿using UnityEngine;

namespace MM.Systems.InventorySystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New RingItem preset", menuName = "ScriptableObjects/MM InventorySystem/RingItemPreset")]
    public class RingItemPreset : ItemPreset
    {
        [Header("Ring")]
        public RingType ringType;


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

        void Reset()
        {
            // Set default values
            type = ItemType.Ring;
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

    public enum RingType
    {
        None,
    }
}
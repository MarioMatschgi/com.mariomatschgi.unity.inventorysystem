﻿#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MM.Systems.InventorySystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InventoryUi))]
    public class InventoryUiEditor : Editor
    {
        // Private
        int inspectButtonHeight = 30;


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Inspect Inventory content", GUILayout.Height(inspectButtonHeight)))
            {
                InventoryUiInspectionWindow.Open((InventoryUi)target);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            base.OnInspectorGUI();
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
}

#endif
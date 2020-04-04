#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MM.Systems.InventorySystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractorInventoryUi))]
    public class InteractorInventoryUiEditor : Editor
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
            float _width = Screen.width / 2;

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Inspect Main-Inventory content", GUILayout.Width(_width), GUILayout.Height(inspectButtonHeight)))
            {
                InventoryUiInspectionWindow.Open(((InteractorInventoryUi)target).mainInventory);
            }
            if (GUILayout.Button("Inspect Ring-Inventory content", GUILayout.Width(_width), GUILayout.Height(inspectButtonHeight)))
            {
                InventoryUiInspectionWindow.Open(((InteractorInventoryUi)target).ringInventory);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Inspect Armor-Inventory content", GUILayout.Width(_width), GUILayout.Height(inspectButtonHeight)))
            {
                InventoryUiInspectionWindow.Open(((InteractorInventoryUi)target).armorInventory);
            }
            if (GUILayout.Button("Inspect Hotbar-Inventory content", GUILayout.Width(_width), GUILayout.Height(inspectButtonHeight)))
            {
                InventoryUiInspectionWindow.Open(((InteractorInventoryUi)target).hotbarInventory);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);

            base.OnInspectorGUI();
        }

        #endregion

        #region Gameplay Methodes
        /*
         *
         * 
         *  Gameplay Methodes
         *
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
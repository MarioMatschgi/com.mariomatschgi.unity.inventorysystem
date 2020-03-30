﻿#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InventoryUiInspectionWindow : EditorWindow
{
    // Public static
    public static InventoryUiInspectionWindow window { get; private set; }


    // Private static
    static Vector2 emptyInvSize = new Vector2(300, 30);
    static InventoryUi inventoryUi;

    static float slotMargin = 10;
    static Vector2 slotSize = new Vector2(100, 100);


    // Public
    public float itemLabelWidth = 30;
    public float amountLabelWidth = 50;

    // Private
    Vector2 verticalScrollPos;


    #region Callback Methodes
    /*
     *
     *  Callback Methodes
     * 
     */

    public static void Open(InventoryUi _inventoryUi)
    {
        // Setup specific variables
        inventoryUi = _inventoryUi;

        // Setup
        Setup();
    }
    static void Setup()
    {
        // If a InventoryUiInspectionWindow is open, close it
        if (window != null)
            window.Close();

        // Create the window
        window = GetWindow<InventoryUiInspectionWindow>();
        window.titleContent = new GUIContent("InventoryUi content viewer");

        // Show the window
        window.Show();
        RectOffset _defaultMargin = EditorStyles.inspectorDefaultMargins.padding;

        if (inventoryUi.spaceY <= 0 || inventoryUi.spaceX <= 0)
            window.minSize = emptyInvSize;
        else
            window.minSize = new Vector2(inventoryUi.spaceX * (slotSize.x + slotMargin + _defaultMargin.horizontal),
                inventoryUi.spaceY * (slotSize.y + slotMargin + _defaultMargin.vertical) + 4 * _defaultMargin.vertical);
        window.maxSize = window.minSize;

        window.minSize = emptyInvSize;
        window.maxSize = Vector2.positiveInfinity;
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

    void OnGUI()
    {
        // Draw Error Label and return
        if (inventoryUi == null)
        {
            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Label("No inventory was selected", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            return;
        }


        // Draw Error Label and return
        if (inventoryUi.spaceY <= 0 || inventoryUi.spaceX <= 0)
        {
            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Label("The inventory is empty or not setup", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            return;
        }


        // Cache
        ItwItemData[][] _cachedDatas = inventoryUi.items;
        if (_cachedDatas == null)
        {
            _cachedDatas = new ItwItemData[inventoryUi.spaceY][];
            for (int i = 0; i < _cachedDatas.Length; i++)
                _cachedDatas[i] = new ItwItemData[inventoryUi.spaceX];
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical();
        verticalScrollPos = EditorGUILayout.BeginScrollView(verticalScrollPos);

        int _row = 0;
        foreach (ItwItemData[] _itemDataList in _cachedDatas)
        {
            _row++;
            int _column = 0;

            EditorGUILayout.BeginHorizontal();
            foreach (ItwItemData _itemData in _itemDataList)
            {
                _column++;

                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal();
                GUIStyle _labelStyle = EditorStyles.boldLabel;
                _labelStyle.alignment = TextAnchor.UpperLeft;
                GUIStyle _valueStyle = EditorStyles.wordWrappedLabel;
                _valueStyle.alignment = TextAnchor.UpperLeft;

                EditorGUILayout.LabelField("Item:", _labelStyle, GUILayout.Width(itemLabelWidth), GUILayout.Height(slotSize.y - _valueStyle.lineHeight));
                if (_itemData == null || _itemData.itemPreset == null)
                    EditorGUILayout.LabelField("Null", _valueStyle, GUILayout.Width(slotSize.x - itemLabelWidth), GUILayout.Height(slotSize.y - _valueStyle.lineHeight));
                else
                    EditorGUILayout.LabelField(_itemData.itemPreset.name, _valueStyle, GUILayout.Width(slotSize.x - itemLabelWidth), GUILayout.Height(slotSize.y - _valueStyle.lineHeight));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Amount:", _labelStyle, GUILayout.Width(amountLabelWidth), GUILayout.Height(_valueStyle.lineHeight));
                if (_itemData == null || _itemData.itemPreset == null)
                    EditorGUILayout.LabelField("0", _valueStyle, GUILayout.Width(slotSize.x - amountLabelWidth), GUILayout.Height(_valueStyle.lineHeight));
                else
                    EditorGUILayout.LabelField(_itemData.itemAmount.ToString(), _valueStyle, GUILayout.Width(slotSize.x - amountLabelWidth), GUILayout.Height(_valueStyle.lineHeight));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                if (_column < inventoryUi.spaceX)
                    EditorGUILayout.Space(slotMargin);
            }
            EditorGUILayout.EndHorizontal();

            if (_row < inventoryUi.spaceY)
                EditorGUILayout.Space(slotMargin);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }

    #endregion

    #region Helper Methodes
    /*
     *
     *  Helper Methodes
     * 
     */

    #endregion
}

#endif
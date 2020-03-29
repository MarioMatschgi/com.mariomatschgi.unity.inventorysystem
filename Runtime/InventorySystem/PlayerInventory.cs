using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("General")]
    public int inventorySpaceX = 10;
    public int inventorySpaceY = 3;
    [Space]
    public int armorSpaceX = 2;
    public int armorSpaceY = 3;
    [Space]
    public int ringSpaceX = 2;
    public int ringSpaceY = 3;
    [Space]
    public bool isInventoryOpen;
    public bool wasInventoryOpen;


    public ItwItemData[][] items { get; private set; }
    public ItwItemData[][] armorItems { get; private set; }
    public ItwItemData[][] ringItems { get; private set; }


    public delegate void OnInventoryChangedEvent();
    public OnInventoryChangedEvent inventoryChangedCallback;


    #region Callback Methodes
    /*
     *
     *  Callback Methodes
     * 
     */

    void Awake()
    {
        // Setup Variables
        items = new ItwItemData[inventorySpaceY][];
        for (int i = 0; i < items.Length; i++)
            items[i] = new ItwItemData[inventorySpaceX];
    }

    void Start()
    {
        
    }

    void LateUpdate()
    {
        wasInventoryOpen = isInventoryOpen;
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

    /// <summary>
    /// Adds an item <paramref name="_newItem"/> to the list
    /// </summary>
    /// <param name="_newItem"></param>
    /// <returns>Returns the new item amount</returns>
    public int AddItem(Item _newItem)
    {
        bool _wasInventoryChanged = false;
        int _newAmt = _newItem.itemData.itemAmount;


        //for (int i = 0; i < items.Count; i++)
        //{
        //    //List<ItwItemData> _subList = items[i];

        //    // Try to stack onto any item
        //    for (int j = 0; j < items[i].Count; j++)
        //    {
        //        //ItwItemData _itemData = _subList[j];

        //        if (items[i][j].Equals(_newItem.itemData))
        //        {
        //            int _min = Mathf.Min(Mathf.Abs(items[i][j].itemPreset.stackSize - items[i][j].itemAmount), _newAmt);
        //            items[i][j].itemAmount += _min;
        //            _newAmt -= _min;

        //            _wasInventoryChanged = true;
        //        }
        //    }

        //    // If it couldn't stack try to add newItem to the list
        //    if (items[i].Count <= inventorySpaceX)
        //        if (_newAmt > 0)
        //        {
        //            items[i].Add(new ItwItemData(_newItem.itemData.itemPreset, _newAmt));
        //            _newAmt = 0;

        //            _wasInventoryChanged = true;

        //            // Break, so it doesn't get added multiple times
        //            break;
        //        }
        //}
        for (int i = 0; i < items.Length; i++)
        {
            ItwItemData[] _subList = items[i];

            // Try to stack onto any item
            for (int j = 0; j < items.Length; j++)
            {
                ItwItemData _itemData = _subList[j];

                if (_itemData != null && _itemData.Equals(_newItem.itemData))
                {
                    int _min = Mathf.Min(Mathf.Abs(_itemData.itemPreset.stackSize - _itemData.itemAmount), _newAmt);
                    _itemData.itemAmount += _min;
                    _newAmt -= _min;

                    _wasInventoryChanged = true;
                }
            }

            // If it couldn't stack try to add newItem to the list
            if (_subList.Length <= inventorySpaceX)
                if (_newAmt > 0)
                    for (int j = 0; j < _subList.Length; j++)
                        if (_subList[j] == null)
                        {
                            _subList[j] = new ItwItemData(_newItem.itemData.itemPreset, _newAmt);

                            _newAmt = 0;

                            _wasInventoryChanged = true;

                            // Break, so it doesn't get added multiple times
                            break;
                        }
        }

        // Invoke inventoryChangedCallback
        if (_wasInventoryChanged)
            if (inventoryChangedCallback != null)
                inventoryChangedCallback.Invoke();


        //Debug.Log("PrintStart");
        //foreach (ItwItemData[] _subList in items)
        //    foreach (ItwItemData _itemData in _subList)
        //        Debug.Log("ItemData: " + (_itemData == null ? "null" : _itemData.itemPreset == null ? "air" : _itemData.itemPreset.name));
        //Debug.Log("PrintEnd");


        // Item wasnt added, so return false
        return _newAmt;
    }

    /// <summary>
    /// Removes an item <paramref name="_removeItem"/> from the list
    /// </summary>
    /// <param name="_removeItem"></param>
    /// <returns>True if removing was successful</returns>
    public bool RemoveItem(Item _removeItem)
    {
        /*
        foreach (List<ItwItemData> _subList in items)
        {
            // If subList doesnt contain the item, check the next one
            if (!_subList.Contains(_removeItem))
                continue;

            // Else Remove _removeItem from the list
            _subList.Remove(_removeItem);

            // Invoke inventoryChangedCallback
            if (inventoryChangedCallback != null)
                inventoryChangedCallback.Invoke();

            // Return true for success
            return true;
        }*/

        // Item wasnt removed, so return false
        return false;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class InventoryUi : MonoBehaviour
{
    [Header("General")]
    public InventoryUiSlot[] slots;
    ItwItemData[][] m_items;
    public ItwItemData[][] items
    {
        get
        {
            return m_items;
        }
        set
        {
            m_items = value;

            UpdateSlots();
        }
    }
    [Space]
    public bool isActive;
    [Space]
    public int spaceX;
    public int spaceY;

    [Header("Prefabs")]
    public GameObject slotPrefab;


    private GridLayoutGroup gridLayout;


    #region Callback Methodes
    /*
     *
     *  Callback Methodes
     * 
     */

    public void Setup(int _spaceX, int _spaceY, ItwItemData[][] _items)
    {
        // Setup variables
        spaceX = _spaceX;
        spaceY = _spaceY;

        items = new ItwItemData[_spaceY][];
        for (int i = 0; i < items.Length; i++)
            items[i] = new ItwItemData[_spaceX];
        //items = _items;
        gridLayout = GetComponent<GridLayoutGroup>();

        // Setup grid constraint
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = spaceX;

        // Create Slots
        CreateSlots();

        // Search Slots
        SearchSlots();

        // Update Slots
        UpdateSlots(items);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
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

    public void UpdateSlots(ItwItemData[][] _items = null)
    {
        // Update variables if no new get updated
        if (_items != null)
            items = _items;

        // Update each Slot
        for (int i = 0; i < slots.Length; i++)
        {
            ItwItemData _itemData = null;
            try { _itemData = items[Mathf.FloorToInt(i / spaceX)][i % spaceX]; } catch (System.Exception) { }

            slots[i].itemPos = new Vector2Int(i % spaceX, Mathf.FloorToInt(i / spaceX));
            slots[i].inventoryUi = this;
            slots[i].UpdateSlot(_itemData);
            //Debug.Log("Updating Slot: " + slots[i].name + " with item: " + (_item == null ? "null" : _item.name));
        }
    }

    #endregion

    #region Helper Methodes
    /*
     *
     *  Helper Methodes
     * 
     */

    void CreateSlots()
    {
        int _slotsAmt = spaceX * spaceY;
        int _slotsCt = 0;
        slots = new InventoryUiSlot[_slotsAmt];

        InventoryUiSlot[] _childSlots = transform.GetComponentsInChildren<InventoryUiSlot>();
        for (int i = 0; i < _childSlots.Length; i++)
        {
            InventoryUiSlot _slot = _childSlots[i];
            if (_slot != null)
            {
                // If current slots are less than the total slots, add it to arr
                if (_slotsCt < _slotsAmt)
                {
                    slots[i] = _slot;

                    _slotsCt++;
                }
                // Else destroy it
                else
                {
                    DestroyImmediate(_slot.gameObject);
                }
            }
        }


        for (int i = 0; i < _slotsAmt - _slotsCt; i++)
            Instantiate(slotPrefab, transform);
    }

    void SearchSlots()
    {
        slots = GetComponentsInChildren<InventoryUiSlot>();
    }

    #endregion
}

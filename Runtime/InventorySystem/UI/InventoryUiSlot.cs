using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryUiSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("General")]
    public ItwItemData itemData;
    [Space]
    public InventoryUi inventoryUi;
    public Vector2Int itemPos;

    [Header("Outlets")]
    public Image icon;
    public TMP_Text amountText;


    #region Callback Methodes
    /*
     *
     *  Callback Methodes
     * 
     */

    void Start()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!inventoryUi.isActive)
            return;

        // Invoke slotSelectedCallback
        if (InventoryUiManager.instance.slotSelectedCallback != null)
            InventoryUiManager.instance.slotSelectedCallback.Invoke(this, eventData.button);
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

    public void UpdateSlot(ItwItemData _newItemData)
    {
        itemData = _newItemData;
        if (_newItemData != null && _newItemData.itemAmount <= 0)
            itemData = null;

        // Update item in inventoryUi
        if (inventoryUi != null && inventoryUi.items != null)
            inventoryUi.items[itemPos.y][itemPos.x] = itemData;

        // If the item is null, reset Slot
        if (itemData == null)
        {
            ResetSlot();

            return;
        }

        // Update Image
        if (itemData != null && itemData.itemPreset != null && itemData.itemPreset.sprite != null)
            icon.sprite = itemData.itemPreset.sprite;
        // Update amountText
        amountText.text = itemData.itemAmount.ToString();
    }

    public void ResetSlot()
    {
        // Reset Image
        icon.sprite = null;
        // Reset Text
        amountText.text = "";
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

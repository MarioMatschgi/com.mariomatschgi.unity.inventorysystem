using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    [Header("Item")]
    public ItwItemData itemData;
    public int startAmount;


    #region Callback Methodes
    /*
     *
     *  Callback Methodes
     * 
     */

    void Awake()
    {
        itemData.itemAmount = startAmount;
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

    public override bool OnInteract(Transform _interactor)
    {
        base.OnInteract(_interactor);

        // Try to collect Item if a player interacted with this Item
        IInteractor _iInteractor = _interactor.GetComponent<IInteractor>();
        if (_iInteractor != null)
            CollectItem(_iInteractor);

        return true;
    }

    #endregion

    #region Helper Methodes
    /*
     *
     *  Helper Methodes
     * 
     */

    void CollectItem(IInteractor _iInteractor)
    {
        // ToDo: Collect Item, add it to players inventory and check for complications
        Debug.Log("Try to collect item named: " + itemData.itemPreset.name);

        itemData.itemAmount = _iInteractor.inventoryUi.AddItem(this);
        if (itemData.itemAmount == 0)
            Destroy(gameObject);
    }

    #endregion
}
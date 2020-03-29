using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUi : MonoBehaviour
{
    [Header("General")]
    public int playerId;
    public IInteractor player;
    [Space]
    public float animationTime = .1f;
    public bool isFinishedAnimating;
    [Space]
    public int selectedRowIdx;

    [Header("Inventories")]
    public CanvasGroup content;
    [Space]
    public RectTransform highlightPanel;
    [Space]
    public InventoryUi hotbarInventory;
    public InventoryUi mainInventory;
    public InventoryUi armorInventory;
    public InventoryUi ringInventory;


    #region Callback Methodes
    /*
     *
     *  Callback Methodes
     * 
     */

    void Awake()
    {
        // Setup variables
        IEnumerable enumerable = FindObjectsOfType<MonoBehaviour>().OfType<IInteractor>();
        foreach (IInteractor _iInteractor in enumerable)
            if (_iInteractor.interactorId == playerId)
            {
                player = _iInteractor;
                player.inventoryUi = this;
            }
        isFinishedAnimating = true;

        // Subscribe to inventoryChangedCallback
        player.inventory.inventoryChangedCallback += UpdateInventory;

        // Setup hotbarInventory
        hotbarInventory.Setup(player.inventory.inventorySpaceX, 1, null);
        hotbarInventory.isActive = false;
        // Setup mainInventory
        mainInventory.Setup(player.inventory.inventorySpaceX, player.inventory.inventorySpaceY, player.inventory.items);
        // Setup armorInventory
        armorInventory.Setup(player.inventory.armorSpaceX, player.inventory.armorSpaceY, null);
        // Setup ringInventory
        ringInventory.Setup(player.inventory.ringSpaceX, player.inventory.ringSpaceY, null);

        // Hide inventory
        content.alpha = 0;
    }

    void Start()
    {

    }

    void Update()
    {
        // Manage Hiding/Showing of the Inventory
        Debug.Log("DDDDDDD: " + player.inventory.isInventoryOpen + "  " + player.inventory.wasInventoryOpen);
        if (player.inventory.isInventoryOpen && !player.inventory.wasInventoryOpen && isFinishedAnimating)
            StartCoroutine(OpenInventory());
        else if (!player.inventory.isInventoryOpen && player.inventory.wasInventoryOpen && isFinishedAnimating)
            StartCoroutine(CloseInventory());

        // Limit selectedRowIdx
        if (selectedRowIdx >= player.inventory.inventorySpaceY)
            selectedRowIdx %= player.inventory.inventorySpaceY;
        selectedRowIdx = Mathf.Clamp(selectedRowIdx, 0, player.inventory.inventorySpaceY);

        // Update Hotbar
        hotbarInventory.UpdateSlots(new ItwItemData[][] { mainInventory.items[selectedRowIdx] } );

        // Position highlightPanel
        highlightPanel.position = new Vector3(highlightPanel.position.x, mainInventory.slots[selectedRowIdx * player.inventory.inventorySpaceX].transform.position.y, highlightPanel.position.z);
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

    IEnumerator OpenInventory()
    {
        // Set isFinishedAnimating
        isFinishedAnimating = false;

        // Fade
        content.FadeIn(animationTime, this, false);

        // Wait til anim is finished
        float _time = animationTime;
        while (_time > 0)
        {
            _time -= Time.deltaTime;
            yield return null;
        }

        // SetInventoriesActive
        SetInventoriesActive(true);

        // Set isFinishedAnimating
        isFinishedAnimating = true;
    }

    IEnumerator CloseInventory()
    {
        // Set isFinishedAnimating
        isFinishedAnimating = false;

        // SetInventoriesActive
        SetInventoriesActive(false);

        // Fade
        content.FadeOut(animationTime, this, false);

        // Wait til anim is finished
        float _time = animationTime;
        while (_time > 0)
        {
            _time -= Time.deltaTime;
            yield return null;
        }

        // Set isFinishedAnimating
        isFinishedAnimating = true;
    }

    void UpdateInventory()
    {
        // Update Main inventory
        mainInventory.UpdateSlots(player.inventory.items);
    }

    void SetInventoriesActive(bool _active)
    {
        mainInventory.isActive = _active;
        armorInventory.isActive = _active;
        ringInventory.isActive = _active;
    }

    #endregion
}

using System.Collections;
using System.Linq;
using UnityEngine;
using MM.Libraries.UI;

namespace MM.Systems.InventorySystem
{
    [AddComponentMenu("MM InventorySystem/Player Inventory UI")]
    [DefaultExecutionOrder(-5)]
    public class InteractorInventoryUi : MonoBehaviour
    {
        // Public
        [Header("General")]
        public int playerId;
        public IInteractor player;
        [Space]
        public InventoryUiSlot hoveringSlot;
        [Space]
        bool m_isInventoryOpen;
        public bool isInventoryOpen
        {
            get
            {
                return m_isInventoryOpen;
            }
            set
            {
                m_isInventoryOpen = value;

                UpdateInventoryVisibility(m_isInventoryOpen);
            }
        }
        [Space]
        public float animationTime = .1f;
        public bool isFinishedAnimating;
        [Space]
        public int hotbarRowIdx;

        [Header("Inventories")]
        public int inventorySpaceX = 10;
        public int inventorySpaceY = 3;
        [Space]
        public int armorSpaceX = 2;
        public int armorSpaceY = 3;
        [Space]
        public int ringSpaceX = 2;
        public int ringSpaceY = 3;
        [Space]
        public CanvasGroup content;
        [Space]
        public RectTransform highlightPanel;
        [Space]
        public InventoryUi hotbarInventory;
        public InventoryUi mainInventory;
        public InventoryUi armorInventory;
        public InventoryUi ringInventory;


        // Delegates
        public delegate void OnInventoryChangedEvent();
        public OnInventoryChangedEvent inventoryChangedCallback;


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/MM InventorySystem/PlayerInventory Panel", false, 10)]
        public static void OnCreate()
        {
#pragma warning disable CS0618 // Type or member is obsolete

            // Create EventSystem
            InventoryUiManager _manager = (InventoryUiManager)FindObjectsOfTypeAll(typeof(InventoryUiManager))[0];
            if (_manager == null || _manager.gameObject == null || _manager.gameObject.scene.name == null || _manager.gameObject.scene.name.Equals(string.Empty))
            {
                InventoryUiManager.OnCreate();

                _manager = (InventoryUiManager)FindObjectsOfTypeAll(typeof(InventoryUiManager))[0];
            }

            // Create PlayerInventory Panel
            GameObject _playerInvObj = (GameObject)Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.mariomatschgi.unity.inventorysystem/Prefabs/PlayerInventoryPanel.prefab",
                typeof(GameObject)), _manager.transform);
            _playerInvObj.transform.SetSiblingIndex(0);
            _playerInvObj.transform.name = "PlayerInventoryPanel";

#pragma warning restore CS0618 // Type or member is obsolete
        }
#endif

        protected virtual void Awake()
        {
            // Setup variables
            IEnumerable _enumerable = FindObjectsOfType<MonoBehaviour>().OfType<IInteractor>();
            foreach (IInteractor _iInteractor in _enumerable)
                if (_iInteractor.interactorId == playerId)
                {
                    player = _iInteractor;
                    player.inventoryUi = this;
                }
            isFinishedAnimating = true;

            // Subscribe to inventoryChangedCallback
            inventoryChangedCallback += UpdateInventory;

            // Setup hotbarInventory
            hotbarInventory.Setup(inventorySpaceX, 1, this);
            hotbarInventory.isActive = false;
            // Setup mainInventory
            mainInventory.Setup(inventorySpaceX, inventorySpaceY, this);
            // Setup armorInventory
            armorInventory.Setup(armorSpaceX, armorSpaceY, this);
            // Setup ringInventory
            ringInventory.Setup(ringSpaceX, ringSpaceY, this);

            // Hide inventory
            content.alpha = 0;
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            // Limit selectedRowIdx
            if (hotbarRowIdx >= inventorySpaceY)
                hotbarRowIdx %= inventorySpaceY;
            hotbarRowIdx = Mathf.Clamp(hotbarRowIdx, 0, inventorySpaceY);

            // Update Hotbar
            hotbarInventory.UpdateSlots(new ItemData[][] { mainInventory.items[hotbarRowIdx] });

            // Position highlightPanel
            highlightPanel.position = new Vector3(highlightPanel.position.x, mainInventory.slots[hotbarRowIdx * inventorySpaceX].transform.position.y, highlightPanel.position.z);
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

        public void TryDropHovered()
        {
            if (hoveringSlot != null && hoveringSlot.itemData != null && hoveringSlot.itemData.itemPreset != null)
            {
                // If Control/command key is pressed, drop full
                if ( ((Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) && Input.GetKey(KeyCode.LeftCommand)) ||
                     ((Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) && Input.GetKey(KeyCode.LeftControl)) )
                {
                    InventoryUiManager.instance.DropItem(hoveringSlot.itemData, player);

                    hoveringSlot.UpdateSlot(null);
                }
                // Else only drop 1 Item
                else
                {
                    // Only drop 1 Item
                    ItemData _newData = new ItemData(hoveringSlot.itemData.itemPreset, 1);
                    InventoryUiManager.instance.DropItem(_newData, player);

                    // Update slot with 1 less
                    _newData = new ItemData(hoveringSlot.itemData.itemPreset, hoveringSlot.itemData.itemAmount - 1);
                    hoveringSlot.UpdateSlot(_newData);
                }
            }
        }

        public void DropIfMouseOutside()
        {
            if (content.gameObject.activeInHierarchy && !RectTransformUtility.RectangleContainsScreenPoint(content.gameObject.GetComponent<RectTransform>(), Input.mousePosition, Camera.current))
                TryDropCursor();
        }

        public void TryDropCursor()
        {
            if (InventoryUiManager.instance.cursorItemSlot.itemData != null && InventoryUiManager.instance.cursorItemSlot.itemData.itemPreset != null)
            {
                InventoryUiManager.instance.DropItem(InventoryUiManager.instance.cursorItemSlot.itemData, player);

                InventoryUiManager.instance.ResetCursor();
            }
        }

        /// <summary>
        /// Adds an item <paramref name="_newItemData"/> to the list
        /// </summary>
        /// <param name="_newItemData"></param>
        /// <returns>Returns the new item amount</returns>
        public virtual int AddItem(ItemData _newItemData, bool _forceMainInv = false)
        {
            bool _wasInventoryChanged = false;
            int _newAmt = _newItemData.itemAmount;

            // Set inventory
            InventoryUi _inv = mainInventory;
            if (!_forceMainInv)
            {
                if (_newItemData.itemPreset is RingItemPreset)
                    _inv = ringInventory;
                if (_newItemData.itemPreset is ArmorItemPreset)
                    _inv = armorInventory;
            }

            if (_inv.items == null)
                _inv.items = new ItemData[_inv.spaceY][];

            for (int i = 0; i < _inv.items.Length; i++)
            {
                ItemData[] _subList = _inv.items[i];
                if (_subList == null)
                    _subList = new ItemData[_inv.spaceX];

                // Try to stack onto any item
                for (int j = 0; j < _subList.Length; j++)
                {
                    ItemData _itemData = _subList[j];

                    if (_itemData != null && _itemData.Equals(_newItemData))
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
                                _subList[j] = new ItemData(_newItemData.itemPreset, _newAmt);

                                _newAmt = 0;

                                _wasInventoryChanged = true;

                                // Break, so it doesn't get added multiple times
                                break;
                            }
            }
            // Only recurse if its not already recursed and inv has not changed
            if (!_forceMainInv && !_wasInventoryChanged)
                AddItem(_newItemData, true);

            if (_wasInventoryChanged)
            {
                // Update Slots
                _inv.UpdateSlots();

                // Invoke inventoryChangedCallback
                if (inventoryChangedCallback != null)
                    inventoryChangedCallback.Invoke();
            }

            // Return items new amount
            return _newAmt;
        }

        /// <summary>
        /// Removes an item <paramref name="_removeItem"/> from the list
        /// </summary>
        /// <param name="_removeItem"></param>
        /// <returns>True if removing was successful</returns>
        public virtual bool RemoveItem(ItemData _removeItem)
        {
            // ToDo: Remove the item

            // Item wasnt removed, so return false
            return false;
        }

        /// <summary>
        /// Updates the inventorys visibility, Opens/Closes the Inventory
        /// </summary>
        /// <param name="_shouldOpen"></param>
        public virtual void UpdateInventoryVisibility(bool _shouldOpen)
        {
            if (!isFinishedAnimating)
                return;

            // Manage Hiding/Showing of the Inventory
            if (_shouldOpen)
                StartCoroutine(OpenInventory());
            else
            {
                StartCoroutine(CloseInventory());

                TryDropCursor();
            }
        }

        #endregion

        #region Helper Methodes
        /*
         *
         *  Helper Methodes
         * 
         */

        /// <summary>
        /// Corroutine for opening the Inventory
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OpenInventory()
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

        /// <summary>
        /// Corroutine for closing the Inventory
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CloseInventory()
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

        [HideInInspector]
        public bool hasNotChangedInv;
        /// <summary>
        /// Updates the InteractorInventory
        /// </summary>
        protected virtual void UpdateInventory()
        {
            if (hasNotChangedInv)
            {
                hasNotChangedInv = false;

                return;
            }

            // Update hotbarInventory
            hotbarInventory.UpdateSlots();
            // Update mainInventory
            mainInventory.UpdateSlots();
            // Update armorInventory
            armorInventory.UpdateSlots();
            // Update ringInventory
            ringInventory.UpdateSlots();
        }

        /// <summary>
        /// Sets the Inventorys active if <paramref name="_active"/>
        /// </summary>
        /// <param name="_active"></param>
        protected virtual void SetInventoriesActive(bool _active)
        {
            mainInventory.isActive = _active;
            armorInventory.isActive = _active;
            ringInventory.isActive = _active;
        }

        #endregion
    }
}

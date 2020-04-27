using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MM.Systems.InventorySystem
{
    [AddComponentMenu("MM InventorySystem/Inventory UI Manager")]
    public class InventoryUiManager : MonoBehaviour
    {
        public ItemData testData;
        [Header("General")]
        public bool forceDisableInvSetup;
        public bool setupInteractorInventoriesDynamic = true;
        public int startInteractorInventories;
        [Space]
        public InventoryUiSlot firstSelectedSlot;
        [Space]
        public Vector3 itemDropForce = new Vector3(1, 2, 0);

        [Header("Outlets")]
        public InventoryUiSlot cursorItemSlot;

        [Header("Prefabs")]
        public GameObject playerInventoryUiPrefab;
        public GameObject emptyItemPrefab;


        public delegate void OnSlotSelectedEvent(InventoryUiSlot _selectedSlot, PointerEventData.InputButton _button);
        public OnSlotSelectedEvent slotSelectedCallback;


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/MM InventorySystem/Inventory Ui Manager", false, 10)]
        public static void OnCreate()
        {
#pragma warning disable CS0618 // Type or member is obsolete

            // Create EventSystem
            Object[] _objects = FindObjectsOfTypeAll(typeof(EventSystem));
            if (_objects.Length > 0 && _objects[0] == null)
                _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)).GetComponent<EventSystem>();

            // Create InventoryCanvas
            GameObject _invManagerObj = (GameObject)Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.mariomatschgi.unity.inventorysystem/Prefabs/InventoryUiManager.prefab",
                typeof(GameObject)));
            _invManagerObj.transform.name = "InventoryCanvas";

#pragma warning restore CS0618 // Type or member is obsolete
        }
#endif

        public static InventoryUiManager instance { get; private set; }
        void Awake()
        {
            #region Singleton

            // Singleton
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            #endregion

            // Subscribe to slotSelectedCallback
            slotSelectedCallback += OnSlotSelected;

            // Disable CursorItem
            cursorItemSlot.gameObject.SetActive(false);
            // Reset cursorItemSlot
            cursorItemSlot.UpdateSlot(null);

            // Setup InteractorInventories
            SetupInteractorInventories();
        }

        void Start()
        {

        }

        /// <summary>
        /// OnSlotSelected Callback for InventoryUi
        /// </summary>
        /// <param name="_selectedSlot"></param>
        /// <param name="_button"></param>
        void OnSlotSelected(InventoryUiSlot _selectedSlot, PointerEventData.InputButton _button)
        {
            // Manage Swaping/Stacking
            switch (_button)
            {
                case PointerEventData.InputButton.Left:
                    // If its the first click
                    if (firstSelectedSlot == null)
                    {
                        // If selected slot doesn't contain an item, return
                        if (_selectedSlot.itemData == null)
                            return;

                        // First click on slot with item

                        // Set firstSelectedSlot to selectedSlot
                        firstSelectedSlot = _selectedSlot;
                        // Move item to cursor
                        cursorItemSlot.itemPos = _selectedSlot.itemPos;
                        SwapSlotItems(_selectedSlot, cursorItemSlot);
                    }
                    // Else its the second click
                    else
                    {
                        // If selected Slot is empty
                        if (_selectedSlot.itemData == null)
                        {
                            // Only move item if newItemData is allowed to be put in its new inventory
                            if (CheckItemTypeValid(_selectedSlot.inventoryUi, cursorItemSlot.itemData))
                            {
                                // Put cursor item there
                                cursorItemSlot.itemPos = _selectedSlot.itemPos;
                                SwapSlotItems(_selectedSlot, cursorItemSlot);

                                // Reset firstSelectedSlot
                                firstSelectedSlot = null;

                                // Reset cursor
                                ResetCursor();
                            }
                        }
                        // Else 
                        else
                        {
                            // Only move if: CursorItem is allowed in the clicked inv AND selectedItem is allowed in the first selected inv
                            if (CheckItemTypeValid(_selectedSlot.inventoryUi, cursorItemSlot.itemData) && CheckItemTypeValid(firstSelectedSlot.inventoryUi, _selectedSlot.itemData))
                            {
                                // If the selected slot is full, swap the items
                                if (_selectedSlot.itemData.itemAmount >= _selectedSlot.itemData.itemPreset.stackSize)
                                {
                                    // Put cursor item there
                                    cursorItemSlot.itemPos = _selectedSlot.itemPos;
                                    SwapSlotItems(_selectedSlot, cursorItemSlot);

                                    // Set firstSelectedSlot and secondSelectedSlot
                                    firstSelectedSlot = _selectedSlot;
                                }
                                // Else, stack the items
                                else
                                {
                                    // Stack the items
                                    StackSlotItems(cursorItemSlot, _selectedSlot, cursorItemSlot.itemData.itemAmount);

                                    // If cursor amount is 0, reset cursor
                                    if (cursorItemSlot.itemData == null || cursorItemSlot.itemData.itemAmount <= 0)
                                        ResetCursor();
                                }
                            }
                        }
                    }

                    break;

                case PointerEventData.InputButton.Right:
                    // If its the first click
                    if (firstSelectedSlot == null)
                    {
                        if (_selectedSlot.itemData != null)
                        {
                            // First click on slot with item

                            // Set firstSelectedSlot
                            firstSelectedSlot = _selectedSlot;
                            // Give half the amount to cursor (Ceil, if odd number, give cursor the most)
                            cursorItemSlot.UpdateSlot(new ItemData(_selectedSlot.itemData.itemPreset, Mathf.CeilToInt((float)_selectedSlot.itemData.itemAmount / 2)));
                            // Give half the amount to selectedSlot (Floor, if odd number, give cuselected slot the least)
                            _selectedSlot.UpdateSlot(new ItemData(_selectedSlot.itemData.itemPreset, Mathf.FloorToInt((float)_selectedSlot.itemData.itemAmount / 2)));
                        }
                    }
                    else
                    {
                        // Try to stack 1 item from cursor to selectedSlot
                        StackSlotItems(cursorItemSlot, _selectedSlot, 1);

                        // If cursor is empty, reset
                        if (cursorItemSlot.itemData == null || cursorItemSlot.itemData.itemAmount <= 0)
                            ResetCursor();
                    }

                    break;
            }
        }

        void Update()
        {
            // If item is currently being moved
            if (cursorItemSlot != null && cursorItemSlot.itemData != null)
            {
                // Set cursorItem active
                if (!cursorItemSlot.gameObject.activeSelf)
                    cursorItemSlot.gameObject.SetActive(true);

                // Move cursorItem to cursor
                cursorItemSlot.transform.position = Input.mousePosition + new Vector3(-10, 10, 0);
            }
            else
            {
                // Set cursorItem deactive
                if (cursorItemSlot.gameObject.activeSelf)
                    cursorItemSlot.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Gameplay Methodes
        /*
         *
         *  Gameplay Methodes
         *
         */

        public void DropItem(ItemData _itemData, IInteractor _interactor)
        {
            // Setup variables
            GameObject _itemGo = Instantiate(emptyItemPrefab, ((MonoBehaviour)_interactor).transform.position, Quaternion.identity);
            Item _item = _itemGo.GetComponent<Item>();

            // Setup
            _item.itemData = _itemData;
            _item.startAmount = _itemData.itemAmount;
            _item.SetupDrop(_interactor);

        }

        /// <summary>
        /// Checks if itemData can be placed in a InventoryUiSlot, returns true if
        /// </summary>
        /// <returns></returns>
        public bool CheckItemTypeValid(InventoryUi _inv, ItemData _newData)
        {


            return false;
        }

        /// <summary>
        /// Resets the cursor Slot
        /// </summary>
        public void ResetCursor()
        {
            // Reset Cursor
            cursorItemSlot.UpdateSlot(null);
            // Reset firstSelectedSlot
            firstSelectedSlot = null;
        }

        /// <summary>
        /// Swaps two Slots, first gets cached
        /// </summary>
        /// <param name="_firstSlot"></param>
        /// <param name="_secondSlot"></param>
        public void SwapSlotItems(InventoryUiSlot _firstSlot, InventoryUiSlot _secondSlot)
        {
            ItemData _tmpData = _secondSlot.itemData;
            _secondSlot.UpdateSlot(_firstSlot.itemData);
            _firstSlot.UpdateSlot(_tmpData);
        }

        /// <summary>
        /// Stacks the second ontop of the first with a max amount of <paramref name="_maxAmount"/>
        /// </summary>
        /// <param name="_firstSlot"></param>
        /// <param name="_secondSlot"></param>
        /// <param name="_maxAmount"></param>
        public void StackSlotItems(InventoryUiSlot _firstSlot, InventoryUiSlot _secondSlot, int _maxAmount)
        {
            // If first and second slot items dont match, return
            if (_firstSlot.itemData != null && _secondSlot.itemData != null && !_secondSlot.itemData.Equals(_firstSlot.itemData))
                return;

            // Return if second slot is full
            if (_secondSlot.itemData != null && _secondSlot.itemData.itemAmount >= _secondSlot.itemData.itemPreset.stackSize)
                return;

            // Return if first slot is empty
            if (_firstSlot.itemData.itemAmount <= 0)
                return;

            // Calculate amount
            /* Smallest of
             * 1. The amount possible to stack on the second slot (stackSize)
             * 2. The maxAmount
             * 3. The amount possible to stack (first slot amount)
             */
            int _amount = Mathf.Min(_firstSlot.itemData.itemPreset.stackSize - (_secondSlot.itemData == null ? 0 : _secondSlot.itemData.itemAmount),
                _maxAmount, _firstSlot.itemData.itemAmount);
            //int _amount = Mathf.Min(_firstSlot.itemData.itemPreset.stackSize, _maxAmount, _firstSlot.itemData.itemAmount, _secondSlot.itemData.itemAmount);

            // Remove amount from first
            _firstSlot.UpdateSlot(new ItemData(_firstSlot.itemData.itemPreset, _firstSlot.itemData.itemAmount - _amount));

            // Give amount to second slot
            // If second slot is empty, setup a new one
            if (_secondSlot.itemData == null)
                _secondSlot.UpdateSlot(new ItemData(_firstSlot.itemData.itemPreset, _amount));
            // Else stack it to the others
            else
                _secondSlot.UpdateSlot(new ItemData(_secondSlot.itemData.itemPreset, _secondSlot.itemData.itemAmount + _amount));
        }

        #endregion

        #region Helper Methodes
        /*
         *
         *  Helper Methodes
         * 
         */

        /// <summary>
        /// Setups the InteractorInventories and manages the amount of InteractorInventories
        /// </summary>
        void SetupInteractorInventories()
        {
            // If forced, return
            if (forceDisableInvSetup)
                return;

            int _amt = startInteractorInventories;
            if (setupInteractorInventoriesDynamic)
                _amt = FindObjectsOfType<MonoBehaviour>().OfType<IInteractor>().Count();

            for (int i = 0; i < transform.childCount; i++)
            {
                // Get Child
                Transform _transform = transform.GetChild(i);
                if (_transform.GetComponent<InteractorInventoryUi>() == null)
                    continue;

                // If no Invs are left, destroy all next
                if (_amt <= 0)
                    Destroy(_transform.gameObject);

                // Decrease amount left
                _amt--;
            }

            // If amount is smaller than 0, all Invs were inited, so return
            if (_amt < 0)
                return;

            // Else create new Invs
            for (int i = 0; i < Mathf.Abs(_amt); i++)
            {
                // Create
                GameObject _invGO = Instantiate(playerInventoryUiPrefab, transform);
                _invGO.gameObject.name = "PlayerInventoryPanel";
                _invGO.transform.SetSiblingIndex(0);
            }
        }

        #endregion
    }
}
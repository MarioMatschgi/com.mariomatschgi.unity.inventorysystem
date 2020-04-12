using UnityEngine;
using UnityEngine.UI;

namespace MM.Systems.InventorySystem
{
    [RequireComponent(typeof(GridLayoutGroup))]
    [AddComponentMenu("MM InventorySystem/Inventory UI")]
    public class InventoryUi : MonoBehaviour
    {
        [Header("General")]
        public InventoryUiSlot[] slots;
        public ItemData[][] items;
        [Space]
        public bool isActive;
        [Space]
        public int spaceX;
        public int spaceY;
        [Space]
        InteractorInventoryUi interactorInv;

        [Header("Prefabs")]
        public GameObject slotPrefab;


        private GridLayoutGroup gridLayout;


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/MM InventorySystem/Inventory UI", false, 10)]
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
            GameObject _playerInvObj = (GameObject)Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.mariomatschgi.unity.inventorysystem/Prefabs/InventoryUi.prefab",
                typeof(GameObject)), _manager.transform);
            _playerInvObj.transform.name = "InventoryUi";

#pragma warning restore CS0618 // Type or member is obsolete
        }
#endif

        /// <summary>
        /// Sets up the InventoryUi with <paramref name="_spaceX"/> columns and <paramref name="_spaceY"/> rows
        /// </summary>
        /// <param name="_spaceX"></param>
        /// <param name="_spaceY"></param>
        public void Setup(int _spaceX, int _spaceY, InteractorInventoryUi _interactorInv = null)
        {
            // Setup variables
            spaceX = _spaceX;
            spaceY = _spaceY;
            interactorInv = _interactorInv;

            items = new ItemData[_spaceY][];
            for (int i = 0; i < items.Length; i++)
                items[i] = new ItemData[_spaceX];
            //items = _items;
            gridLayout = GetComponent<GridLayoutGroup>();

            // Setup grid constraint
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = spaceX;

            // Create Slots
            SetupSlots();

            // Search Slots
            slots = GetComponentsInChildren<InventoryUiSlot>();

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
         *  Gameplay Methodes
         *
         */

        /// <summary>
        /// Updates the Slots of this InventoryUi
        /// </summary>
        /// <param name="_items"></param>
        public void UpdateSlots(ItemData[][] _items = null)
        {
            bool _hasChanged = _items == null ? (items == null ? false : true) : (!_items.Equals(items) && _items == items);

            // Update variables if no new get updated
            if (_items != null)
                items = _items;

            // Update each Slot
            for (int i = 0; i < slots.Length; i++)
            {
                ItemData _itemData = null;
                try { _itemData = items[Mathf.FloorToInt(i / spaceX)][i % spaceX]; } catch (System.Exception) { }

                slots[i].itemPos = new Vector2Int(i % spaceX, Mathf.FloorToInt(i / spaceX));
                slots[i].inventoryUi = this;
                slots[i].UpdateSlot(_itemData);
                //Debug.Log("Updating Slot: " + slots[i].name + " with item: " + (_item == null ? "null" : _item.name));
            }

            if (_hasChanged && interactorInv != null)
            {
                interactorInv.hasNotChangedInv = true;

                if (interactorInv.inventoryChangedCallback != null)
                    interactorInv.inventoryChangedCallback.Invoke();
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
        /// Setups the Slots, manages the amount of Slots and updates with default value
        /// </summary>
        void SetupSlots()
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

        #endregion
    }
}
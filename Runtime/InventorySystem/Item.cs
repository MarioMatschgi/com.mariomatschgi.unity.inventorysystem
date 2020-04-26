using System.Collections;
using UnityEngine;

namespace MM.Systems.InventorySystem
{
    [AddComponentMenu("MM InventorySystem/Item")]
    public class Item : Interactable
    {
        // Static
        static bool logTryCollect = false;


        [Header("Item")]
        public ItemData itemData;
        public int startAmount;


        Rigidbody2D rb;
        float tmpXPos;
        float tmpYPos;
        bool isDropping;


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/MM InventorySystem/Item", false, 10)]
        public static void OnCreate()
        {
            // Create Item
            GameObject _invManagerObj = (GameObject)Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.mariomatschgi.unity.inventorysystem/Prefabs/Item.prefab",
                typeof(GameObject)));
            _invManagerObj.transform.name = "Item";
        }

        [UnityEditor.MenuItem("GameObject/MM InventorySystem/Item 2D", false, 10)]
        public static void OnCreate2D()
        {
            // Create Item
            GameObject _invManagerObj = (GameObject)Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.mariomatschgi.unity.inventorysystem/Prefabs/Item2D.prefab",
                typeof(GameObject)));
            _invManagerObj.transform.name = "Item2D";
        }
#endif

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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
         *  Gameplay Methodes
         *
         */

        /// <summary>
        /// OnInteract Callback for Items
        /// </summary>
        /// <param name="_interactor"></param>
        /// <returns>True if interaction was successful</returns>
        public override bool OnInteract(Transform _interactor)
        {
            if (isDropping)
                return false;

            base.OnInteract(_interactor);

            // Try to collect Item if a interactor interacted with this Item
            IInteractor _iInteractor = _interactor.GetComponent<IInteractor>();
            if (_iInteractor != null)
                CollectItem(_iInteractor);

            return true;
        }

        public void SetupDrop(IInteractor _interactor)
        {
            tmpYPos = transform.position.y;
            tmpXPos = transform.position.x;
            rb.velocity = Vector2.zero;
            Vector2 _force = InventoryUiManager.instance.itemDropForce;
            // Invert X if interactor is facing -X
            if (((MonoBehaviour)_interactor).transform.forward.x < 0)
                _force.x *= -1;

            rb.AddForce(_force, ForceMode2D.Impulse);

            StartCoroutine(SetupDropIE());
        }

        #endregion

        #region Helper Methodes
        /*
         *
         *  Helper Methodes
         * 
         */

        IEnumerator SetupDropIE()
        {
            isDropping = true;
            rb.gravityScale = 1;

            while (Mathf.Approximately(transform.position.y, tmpYPos) || transform.position.y >= tmpYPos)
                yield return null;

            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            isDropping = false;
        }

        /// <summary>
        /// Collects this Item
        /// </summary>
        /// <param name="_iInteractor"></param>
        void CollectItem(IInteractor _iInteractor)
        {
            if (logTryCollect)
                Debug.Log("Try to collect item named: " + itemData.itemPreset.name);

            itemData.itemAmount = _iInteractor.inventoryUi.AddItem(itemData);
            if (itemData.itemAmount == 0)
                Destroy(gameObject);
        }

        #endregion
    }
}
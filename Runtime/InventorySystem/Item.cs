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


        new Collider collider;
        Rigidbody rb;
        new Collider2D collider2D;
        Rigidbody2D rb2D;
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
            rb2D = GetComponent<Rigidbody2D>();
            rb = GetComponent<Rigidbody>();
            collider2D = GetComponent<Collider2D>();
            collider = GetComponent<Collider>();
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
            // Setup
            if (collider2D != null)
                collider2D.enabled = false;
            if (collider != null)
                collider.enabled = false;

            tmpYPos = transform.position.y;
            tmpXPos = transform.position.x;
            if (rb2D != null)
                rb2D.velocity = Vector2.zero;
            if (rb != null)
                rb.velocity = Vector3.zero;

            Vector3 _force = InventoryUiManager.instance.itemDropForce;
            // Invert X if interactor is facing -X
            if (((MonoBehaviour)_interactor).transform.forward.x < 0)
                _force.x *= -1;

            // Add force
            if (rb2D != null)
                rb2D.AddForce(_force, ForceMode2D.Impulse);
            if (rb != null)
                rb.AddForce(_force, ForceMode.Impulse);

            // Start Corroutine
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

            if (rb2D != null)
                rb2D.gravityScale = 1;
            if (rb != null)
                rb.useGravity = true;

            while (Mathf.Approximately(transform.position.y, tmpYPos) || transform.position.y >= tmpYPos)
                yield return null;

            rb2D.velocity = Vector2.zero;
            if (rb2D != null)
                rb2D.gravityScale = 0;
            if (rb != null)
                rb.useGravity = false;

            if (collider2D != null)
                collider2D.enabled = true;
            if (collider != null)
                collider.enabled = true;
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
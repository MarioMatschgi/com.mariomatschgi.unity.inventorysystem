using UnityEngine;

namespace MM.Systems.InventorySystem
{
    [AddComponentMenu("MM InventorySystem/Example Player")]
    public class ExamplePlayer : MonoBehaviour, IInteractor
    {
        [SerializeField]
        private int playerId;
        public int interactorId { get { return playerId; } set { playerId = value; } }
        public InteractorInventoryUi inventoryUi { get; set; }


        #region Callback Methodes
        /*
         *
         *  Callback Methodes
         * 
         */

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/MM InventorySystem/Example Player", false, 10)]
        public static void OnCreate()
        {
            // Create ExamplePlayer
            GameObject _invManagerObj = (GameObject)Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.mariomatschgi.unity.inventorysystem/Prefabs/ExamplePlayer.prefab",
                typeof(GameObject)));
            _invManagerObj.transform.name = "ExamplePlayer";
        }

        [UnityEditor.MenuItem("GameObject/MM InventorySystem/Example Player 2D", false, 10)]
        public static void OnCreate2D()
        {
            // Create ExamplePlayer
            GameObject _invManagerObj = (GameObject)Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Packages/com.mariomatschgi.unity.inventorysystem/Prefabs/ExamplePlayer2D.prefab",
                typeof(GameObject)));
            _invManagerObj.transform.name = "ExamplePlayer2D";
        }
#endif

        void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {
            // Open / Close inventory
            if (Input.GetKeyDown(KeyCode.E))
                inventoryUi.isInventoryOpen = !inventoryUi.isInventoryOpen;

            // Update hotbar index
            if (Input.GetKeyDown(KeyCode.Tab))
                inventoryUi.hotbarRowIdx++;
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

        #endregion
    }
}
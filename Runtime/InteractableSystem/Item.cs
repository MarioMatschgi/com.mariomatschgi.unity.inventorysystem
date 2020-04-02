using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MM
{
    namespace Libraries
    {
        namespace InventorySystem
        {
            [AddComponentMenu("MM InventorySystem/Item")]
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
        }
    }
}
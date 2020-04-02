using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MM
{
    namespace Libraries
    {
        namespace InventorySystem
        {
            public interface IInteractor
            {
                int interactorId { get; set; }

                PlayerInventoryUi inventoryUi { get; set; }
            }
        }
    }
}
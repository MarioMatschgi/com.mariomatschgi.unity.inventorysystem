using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractor
{
    int interactorId { get; set; }

    PlayerInventory inventory { get; }
    PlayerInventoryUi inventoryUi { get; set; }

    //void Test();
}

//public static class IInteractorExtention
//{
//    public static void Test(this IInteractor _interactor)
//    {
//        Debug.Log("Test");
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageSystem : MonoBehaviour
{
    public int MaxCapasity;
    public int usedCapasity;
    public int currentStock;
    public bool HasStock;

    [Header("Apples")]
    public int ItemID1;
    [Header("Candy")]
    public int ItemID2;
    [Header("TBD")]
    public int ItemID3;//
    public int ItemID4;//
    public int ItemID5;//
    public int ItemID6;//
    public int ItemID7;//
    public int ItemID8;//

    public int[] Id;
     

    private void Update()
    {
        usedCapasity = ItemID1 + ItemID2 + ItemID3 + ItemID4 + ItemID5 + ItemID6 + ItemID7 + ItemID8;

        currentStock = MaxCapasity - usedCapasity;

        if(usedCapasity >= 1)
        {
            HasStock = true;
        }
        else
        {
            HasStock = false;
        }

        ItemID1 = Id[0];
        ItemID2 = Id[1];
        ItemID3 = Id[2];
        Id[3] = ItemID4;
        Id[4] = ItemID5;
        Id[5] = ItemID6;
        Id[6] = ItemID7;
        Id[7] = ItemID8;
    }

    public void AddStockItemID1()
    {
        ItemID1++;
    }// 1
    public void AddStockItemID2()
    {
        ItemID2++;
    }// 2
    public void AddStockItemID3()
    {
        ItemID3++;
    }// 3
    public void AddStockItemID4()
    {
        ItemID4++;
    }// 4
    public void AddStockItemID5()
    {
        ItemID5++;
    }// 5
    public void AddStockItemID6()
    {
        ItemID6++;
    }// 6        
    public void AddStockItemID7()
    {
        ItemID7++;
    }// 7
    public void AddStockItemID8()
    {
        ItemID8++;
    }// 8
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    [Header("values")]
    [SerializeField] private int racks;
    public int emptyRacks;
    public int usedRacks;
    public int desiredStock;

    [Header("Information")]
    public bool needsRestock;
    [SerializeField] private string ClassName;
    public StationClass stationClass;

    public string takenByWorkerName;
    public string takenByCustomerName;

    public bool isTaken;
    public bool IsTakenByCustomer;


    [Header("Id")]
    public int acceptedID;

    [Header("Price")]
    public int HoldingPrice;

    [Header("Stock")]
    public GameObject[] Chips;

    public void OnEnable()
    {
        racks = stationClass.racks;
        ClassName = stationClass.ClassName;
        acceptedID = stationClass.acceptedID;
    }

    private void Update()
    {

        emptyRacks = racks - usedRacks;
        desiredStock = racks;
        if(usedRacks < desiredStock) { needsRestock = true; }
        else { needsRestock = false; }

        if(isTaken && IsTakenByCustomer)
        {
            takenByWorkerName = "";
            takenByCustomerName = "";
        }

        if (!needsRestock)
        {
            takenByWorkerName = "";
            isTaken = false;
        }

        if(usedRacks == 0)
        {
            takenByCustomerName = "";
            IsTakenByCustomer = false;
        }

        if (takenByWorkerName != "")
        {
            isTaken = true;
        }
        else
        {
            isTaken = false;
        }

        if (takenByCustomerName != "")
        {
            IsTakenByCustomer = true;
        }
        else
        {
            IsTakenByCustomer = false;
        }

        if (usedRacks > racks)
        {
            usedRacks = racks;
        }

        UpdateStock();

    }

    public void UpdateStock()
    {

        for (int i = 0; i < racks; i++)
        {
            Chips[i].SetActive(false);


            for (int x = 0; x < usedRacks; x++)
            {
                Chips[x].SetActive(true);
            }
        }

    }
}

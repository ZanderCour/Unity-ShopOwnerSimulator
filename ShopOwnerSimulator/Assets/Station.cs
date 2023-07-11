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
    public bool isTaken;


    [Header("Id")]
    public int acceptedID;

    [Header("Price")]
    public int HoldingPrice;

    [Header("Stock")]
    public GameObject[] Chips;

    public void Start()
    {
        racks = stationClass.racks;
        ClassName = stationClass.ClassName;
        acceptedID = stationClass.acceptedID;
    }

    private void Update()
    {
        if(takenByWorkerName != "")
        {
            isTaken = true;
        }

        emptyRacks = racks - usedRacks;
        desiredStock = racks;
        if(usedRacks < desiredStock) { needsRestock = true; }
        else { needsRestock = false; }

        if (!needsRestock)
        {
            takenByWorkerName = "";
            isTaken = false;
        }

        if(usedRacks > racks)
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

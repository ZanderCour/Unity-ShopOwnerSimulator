using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class WorkerAI : MonoBehaviour
{
    [Header("Stats")]
    public Stats WorkerStats;
    private float moveSpeed;
    private float loadTime;
    private float Skill;

    [Header("Information")]
    public int StationsAmount;
    public Station[] stations;
    public Station selectedStation;
    public float dstSelectedStation, dstSelectedStorageUnit;
    public bool MatchingStation;
    public bool canMove;
    int index;
    public bool allTasksComplete;
    bool needsNowStation;

    [Header("Stock")]
    public int HoldingStock;
    public int StockLimt;
    public bool HasStock;
    int Crateindex;
    public string stockType;


    [Header("Crates")]
    public StorageSystem selectedCrate;
    public StorageSystem[] Crates;
    public bool isPerformingActions;

    [Header("Stock id")]
    int StockIndex;
    [SerializeField] int LoadingID;
    int i = 0;

    [Header("AI")]
    public NavMeshAgent agent;
    Rigidbody rb;

    public string workingStatus;

    float stocklimtFloat;

    private void Start()
    {
        moveSpeed = WorkerStats.moveSpeed;
        loadTime = WorkerStats.loadTime;
        Skill = WorkerStats.Skill;
        StockLimt = WorkerStats.StockLimt;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.speed = moveSpeed;
        canMove = true;
        StartCoroutine(UpdateEachSecond());
        StartCoroutine(UpdateEachFiveSeconds());
    }

    private void Update()
    {

        if (HoldingStock > 0) { HasStock = true; }
        else { HasStock = false; }
        stations = FindObjectsOfType<Station>();
        Crates = FindObjectsOfType<StorageSystem>();
        StationsAmount = stations.Length;

        if (!allTasksComplete || !selectedCrate.HasStock)
        {
            if (selectedStation == null || selectedStation.emptyRacks == 0 || !MatchingStation) { FindNewStation(); }
        }

        if (!HasStock || allTasksComplete)
        {
            //if(selectedCrate.HasStock && selectedCrate.Id[LoadingID] == 0)
            //{
                //FindNewStation();
            //}

            MoveToStorageCrate();
            workingStatus = "Restocking supplies";
        }
        else if(HasStock && !allTasksComplete)
        {
            if (selectedStation.needsRestock && HasStock && MatchingStation)
            {
                MoveToStation();
                workingStatus = "Moving to selected station";

            }
        }

        if(!selectedStation.needsRestock && HasStock)
        {
            MoveToStorageCrate();
            workingStatus = "Unloading supplies";

        }

        if (allTasksComplete)
        {
            workingStatus = "Done";
        }

        #region Distances
        dstSelectedStation = Vector3.Distance(selectedStation.gameObject.transform.position, transform.position);
        dstSelectedStorageUnit = Vector3.Distance(selectedCrate.gameObject.transform.position, transform.position);
        #endregion

        stocklimtFloat = StockLimt;
        Mathf.Clamp(HoldingStock, 0, stocklimtFloat);



        if (dstSelectedStation == 0 || dstSelectedStation < 0.25f)
        {
            RestockStation();
            Debug.Log("Can restock!");
        }

        if (selectedStation.takenByWorkerName == this.gameObject.name)
        {
            MatchingStation = true;
        }
        else
        {
            MatchingStation = false;
        }
    }

    public void FindNewStation()
    {

        Crateindex = Random.Range(0, Crates.Length);
        selectedCrate = Crates[Crateindex];

        index = Random.Range(0, stations.Length);
        selectedStation = stations[index];

        // Name matching
        if (!selectedStation.isTaken) {
            selectedStation.takenByWorkerName = gameObject.name;
        }

        LoadingID = selectedStation.acceptedID;



    }

    public void MoveToStation()
    {
        if (canMove)
        {
            if (HasStock)
            {
                /*
                Transform targetTransform = selectedStation.transform;
                transform.position = Vector3.MoveTowards(transform.position, targetTransform.transform.position, Time.deltaTime * moveSpeed);
                */
                agent.SetDestination(selectedStation.transform.position);
             
                Debug.DrawLine(this.transform.position, selectedStation.transform.position, Color.green);
            }
        }
    }

    bool restocking;

    public void RestockStation()
    {
        bool matchingID = selectedStation.acceptedID == LoadingID;
        bool CanRestock = selectedStation.needsRestock && HoldingStock > 0;
        bool Unload = HoldingStock <= 0;

        if (Unload)
        {
            restocking = false;
            StopCoroutine(StockStation());
        }
        else
        if (matchingID) {
            Debug.Log("ID Confirmed");
            if (CanRestock) {

                if(!restocking) StartCoroutine(StockStation());
                restocking = true;
                Debug.Log("Restocking :)");
                workingStatus = "Restocking station";


            }
        }
        else
        {
            Debug.Log(this.gameObject.name + " Ids dont match!");
        }
    }


    public IEnumerator StockStation()
    {
        if (selectedStation.needsRestock && HoldingStock > 0)
        {
            yield return new WaitForSeconds(loadTime);
            HoldingStock--;
            selectedStation.usedRacks++;
            
            StartCoroutine(StockStation());
        }

        if (!selectedStation.needsRestock || HoldingStock <= 0) {
            StopCoroutine(StockStation());
        }

    }

    public void MoveToStorageCrate()
    {
        if (canMove)
        {
            /*
        Transform crateTransform = selectedCrate.transform;
        transform.position = Vector3.MoveTowards(transform.position, crateTransform.transform.position, Time.deltaTime * moveSpeed);
        */
            agent.SetDestination(selectedCrate.transform.position);


            Debug.DrawLine(this.transform.position, selectedCrate.transform.position, Color.red);
        }

    }

    int tick = 0;

    public void CollisionTick()
    {
        Debug.Log("Ai has detected collision with a storage unit");
        if (!HasStock)
        {
            if (selectedCrate.Id[LoadingID] > 0)
            {
                Debug.Log("Worker wants to Restock");

                if (tick == 0) StartCoroutine(StartRestock());
                tick = 1;
                restocking = false;
            }
        }
        else
        {
            Debug.Log("Worker wants to Unload");
            StartCoroutine(StartUnload());
            restocking = false;
        }
    }

    IEnumerator StartRestock()
    {
        yield return new WaitForSeconds(1);
        canMove = false;

        yield return new WaitForSeconds(loadTime);
        isPerformingActions = true;

        int stockAmountToTake = 0;
        if(selectedCrate.Id[LoadingID] >= selectedStation.emptyRacks) {
            stockAmountToTake = selectedStation.emptyRacks;
        }
        else {
            stockAmountToTake = selectedCrate.Id[LoadingID];
        }

        HoldingStock += stockAmountToTake;
        selectedCrate.Id[LoadingID] -= HoldingStock;
        workingStatus = "Taking stock";



        yield return new WaitForSeconds(3);
        isPerformingActions = false;
        canMove = true;
        tick = 0;
        
    }

    IEnumerator StartUnload()
    {
        yield return new WaitForSeconds(loadTime);

        LoadingID = selectedStation.acceptedID;
        isPerformingActions = true;

        selectedCrate.Id[LoadingID] += HoldingStock;
        HoldingStock = 0;


        yield return new WaitForSeconds(3);
        isPerformingActions = false;
    }

    IEnumerator UpdateEachSecond()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Ticked 1");
        allTasksComplete = AreAllStationsFull();


        StartCoroutine(UpdateEachSecond());
    }

    IEnumerator UpdateEachFiveSeconds()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Ticked 5");

        if (selectedStation.needsRestock && !HasStock)
        {
            CollisionTick();
        }



        StartCoroutine(UpdateEachFiveSeconds());
    }

    public bool AreAllStationsFull()
    {
        foreach (Station Stations in stations)
        {
            if (Stations.needsRestock)
            {
                return false;
            }
        }
        return true;
    }

}

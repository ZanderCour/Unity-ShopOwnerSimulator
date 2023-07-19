using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class CustomerNPC : MonoBehaviour
{
    [Header("Components")]
    private Station[] HiddenStations;
    [SerializeField] List<Station> stations = new List<Station>();
    [SerializeField] Station SelectedStation;
    [SerializeField] Register[] registers;
    [SerializeField] Register selectedRegister;
    NavMeshAgent agent;


    [Header("Information")]
    private List<int> ChecktedIDS = new List<int>();
    public bool IDMatch;
    public int[] HoldingItems;
    public List<bool> allreadyTaken = new List<bool>();
    public bool MatchingStationName;
    [Space(15)]

    [HideInInspector] public List<int> ShoppingList = new List<int>();
    [Space(15)]

    public List<int> ShoppingListAmount = new List<int>();
    [Space(15)]

    public int Money;
    [SerializeField] private float thinkTime;
    public bool Done = false;
    [SerializeField] bool Refilling;
    int index;
    int registerIndex;

    [Header("Hidden")]
    public float dstSelectedStation;


    private void Start()
    {
        HiddenStations = FindObjectsOfType<Station>();
        registers = FindObjectsOfType<Register>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = Random.Range(2f, 5f);

        for(int i = 0; i < HiddenStations.Length; i++)
        {
            stations.Add(HiddenStations[i]);
        }

        FetchShoppingList();           
        SelectNextStation();

        SelectedStation.takenByCustomerName = "";

        Money = Random.Range(5, 101);
        thinkTime = Random.Range(1, 6);
    }

    public void Update()
    {
        Done = ShoppingListDone();

        if (!Done)
        {
            if (SelectedStation == null || SelectedStation.usedRacks == 0 || SelectedStation.isTaken || !IDMatch || !MatchingStationName) { SelectNextStation(); }
            else if (SelectedStation.usedRacks > 0 && MatchingStationName && !SelectedStation.isTaken) { MoveToStation(); }
        }
        else if(SelectedStation != null)
        { 
            MoveToRegister();
        }

        dstSelectedStation = Vector3.Distance(SelectedStation.gameObject.transform.position, transform.position);

        if (dstSelectedStation < 0.25f && !Refilling)
        {
            StartCoroutine(TakeFromshelf(thinkTime));
            Refilling = true;
        }
        else if(dstSelectedStation > 0.35f)
        {
            StopCoroutine(TakeFromshelf(0.1f));
        }

        for (int i = 0; i < ShoppingListAmount.Count; i++)
        {
            if (ShoppingListAmount[i] > 0 && HoldingItems[i] > 0)
            {
                if (HoldingItems[i] == ShoppingListAmount[i])
                {
                    allreadyTaken[i] = true;
                }
                else
                {
                    allreadyTaken[i] = false;
                }
            }

        }

        if (SelectedStation != null) {
            Check();
            IDMatch = ShoppingList.Contains(SelectedStation.acceptedID);
        }
    }


    public void SelectNextStation()
    {
        if (!Done)
        {
            index = Random.Range(0, stations.Count);
            SelectedStation = stations[index];

            // Name matching
            if (!SelectedStation.IsTakenByCustomer && !SelectedStation.isTaken) {
                SelectedStation.takenByCustomerName = gameObject.name;
            }

            // Name matching
            if (SelectedStation.takenByCustomerName == this.gameObject.name)
            {
                MatchingStationName = true;
            }
            else
            {
                MatchingStationName = false;
            }

            //Register
            registerIndex = Random.Range(0, registers.Length);
            selectedRegister = registers[registerIndex];
        }
    }

    public void FetchShoppingList()
    {
        for (int i = 0; i < stations.Count; i++)
        {
            if (!ChecktedIDS.Contains(stations[i].acceptedID))
            {
                ChecktedIDS.Add(stations[i].acceptedID);
                ShoppingList.Add(stations[i].acceptedID);

                ShoppingListAmount.Add(0);
                allreadyTaken.Add(false);
            }
        }
    }


    public void ChooseAmountToTake()
    {
        int ID = SelectedStation.acceptedID;
        if (ID == 0)
        {
            //Chips
            AmountToTake = Random.Range(1, 5);
            ShoppingListAmount[0] = AmountToTake;
        }
        if (ID == 1)
        {
            //Candybars
            AmountToTake = Random.Range(1, 5);
            ShoppingListAmount[1] = AmountToTake;
        }
    }

    int AmountToTake;

    private IEnumerator TakeFromshelf(float thinkingtime)
    {
        if (Done)
        {
            Refilling = false;
            yield break;
        }
            

        yield return new WaitForSeconds(1);
        ChooseAmountToTake();
        yield return new WaitForSeconds(thinkingtime);
        HoldingItems[SelectedStation.acceptedID] += AmountToTake;
        SelectedStation.usedRacks -= AmountToTake;
        yield return new WaitForSeconds(3);
        SelectedStation.takenByCustomerName = "";
        SelectNextStation();
        yield return new WaitForSeconds(thinkingtime);
        Refilling = false;
    }

    public void MoveToStation()
    {
        agent.SetDestination(SelectedStation.transform.position);

        Debug.DrawLine(this.transform.position, SelectedStation.transform.position, Color.blue);
    }
    public void MoveToRegister()
    {
        agent.SetDestination(selectedRegister.transform.position);
        if(SelectedStation != null) {
            SelectedStation.takenByCustomerName = "";
        }
        Debug.DrawLine(this.transform.position, selectedRegister.transform.position, Color.white);
    }

    private bool ShoppingListDone()
    {
        for (int i = 0; i < allreadyTaken.Count; ++i)
        {
            if (allreadyTaken[i] == false)
            {
                return false;
            }
        }

        return true;
    }

    private void Check()
    {
        for(int x = 0; x < stations.Count; x++)
        {
            Station station = stations[x];

            if(allreadyTaken[station.acceptedID])
            {
                stations.Remove(station);
            }
        }

        if(stations.Count == 0 && !Done)
        {
            for (int i = 0; i < HiddenStations.Length; i++)
            {
                stations.Add(HiddenStations[i]);
            }
        }

    }
}

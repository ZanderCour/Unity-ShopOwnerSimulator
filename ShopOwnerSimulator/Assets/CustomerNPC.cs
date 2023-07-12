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
    public bool TakenStation;
    [Space(15)]

    public bool ValidStation;

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
            if (SelectedStation == null || SelectedStation.usedRacks == 0 || SelectedStation.isTaken || !IDMatch || !ValidStation || !TakenStation) { SelectNextStation(); }
            else if (SelectedStation.usedRacks > 0) { MoveToStation(); }
        }
        else
        { 
            MoveToRegister();
        }

        dstSelectedStation = Vector3.Distance(SelectedStation.gameObject.transform.position, transform.position);

        if (!Done)
        {
            if (SelectedStation.takenByCustomerName == this.gameObject.name)
            {
                TakenStation = true;
            }
            else
            {
                TakenStation = false;
            }
        }

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

        if (allreadyTaken[SelectedStation.acceptedID])
        {
            ValidStation = false;
        }
        else
        {
            ValidStation = true;
        }

        if (SelectedStation != null) {
            Check();
        }
    }


    public void SelectNextStation()
    {
        if (!Done)
        {
            if(SelectedStation != null)
            {
                IDMatch = ShoppingList.Contains(SelectedStation.acceptedID);
                SelectedStation.takenByCustomerName = "";
            }

            index = Random.Range(0, stations.Count);
            SelectedStation = stations[index];

            registerIndex = Random.Range(0, registers.Length);
            selectedRegister = registers[registerIndex];

            // Name matching
            if (!SelectedStation.IsTakenByCustomer) {
                SelectedStation.takenByCustomerName = gameObject.name;
            }
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
        if (Done || !ValidStation)
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
        if (dstSelectedStation == 0) { SelectNextStation(); }
        yield return new WaitForSeconds(1);
        SelectedStation.IsTakenByCustomer = false;
        yield return new WaitForSeconds(thinkingtime);
        SelectedStation.takenByCustomerName = "";
        Refilling = false;
    }

    public void MoveToStation()
    {
        agent.SetDestination(SelectedStation.transform.position);

        Debug.DrawLine(this.transform.position, SelectedStation.transform.position, Color.green);
    }
    public void MoveToRegister()
    {
        agent.SetDestination(selectedRegister.transform.position);


        Debug.DrawLine(this.transform.position, selectedRegister.transform.position, Color.grey);
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

    }
}

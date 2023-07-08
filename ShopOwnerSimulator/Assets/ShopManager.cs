using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int ValueOfStore;
    public int[] values;

    [SerializeField] Station[] stations;


    //Alting er i Dollars
    [Header("Prices")]
    [Space(10)]
    [Header("Candy & Sweets")]
    public int ChipsPrice = 2; //ID = 0
    public int Candybars = 1; //ID = 1
    public int Candybag = 2; //ID = 2


    private void OnEnable()
    {
        stations = FindObjectsOfType<Station>();

        values[0] = ChipsPrice;
        values[1] = Candybars;
        values[2] = Candybag;

        for(int i = 0; i < stations.Length; i++)
        {

        }

    }
}

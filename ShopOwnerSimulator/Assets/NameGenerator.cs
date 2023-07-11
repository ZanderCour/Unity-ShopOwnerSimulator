using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameGenerator : MonoBehaviour
{
    public List<string> BoyNames = new List<string>();
    public List<string> GirlNames = new List<string>();


    public List<string> LastNames = new List<string>();

    public TextMeshProUGUI Text;
    public bool Worker;
    public string name;
    bool boy;
    bool girl;

    private void Start()
    {
        GenerateName();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GenerateName();
        }
    }

    public void GenerateName()
    {
        int Genderindex = Random.Range(1, 3);
        if (Genderindex == 1)
        {
            boy = true;
            int Firstname = Random.Range(1, BoyNames.Count);
            int Lastname = Random.Range(1, LastNames.Count);
            name = BoyNames[Firstname] + " " + LastNames[Lastname];
        }
        else if (Genderindex == 2)
        {
            girl = true;
            int Firstname = Random.Range(1, GirlNames.Count);
            int Lastname = Random.Range(1, LastNames.Count);
            name = GirlNames[Firstname] + " " + LastNames[Lastname];
        }

        if(Worker)
            Text.text = name;
    }
}

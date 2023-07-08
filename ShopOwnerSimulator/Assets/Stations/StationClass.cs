using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Station classes")]
public class StationClass : ScriptableObject
{
    public int racks;
    [Header("Name identifier")]
    public string ClassName;
    public int acceptedID;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Worker Stats")]
public class Stats : ScriptableObject 
{
    [Header("Floats")]
    public float moveSpeed; //How fast the worker moves
    public float loadTime; //How fast the worker goes though the stations
    public float Skill; //How good they are to determin how much money the player gets
    public int StockLimt; //The limt of how much the worker can carry
    
}

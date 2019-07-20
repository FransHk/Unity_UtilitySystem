using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The boosts that this item contains
/// on being applied to the agent
/// </summary>
public class Item : MonoBehaviour
{
    public float HealthBoost { get; set;}
    public float EnergyBoost { get; set;}
    public float AttackBoost { get; set;}
}

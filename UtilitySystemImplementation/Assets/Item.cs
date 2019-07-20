using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The boosts that this item contains
/// on being applied to the agent
/// </summary>
public class Item : MonoBehaviour, IPossibilityTarget
{
   
    public PossibilityType Type { get => type; set => type = value; }
    public float HealthBoost { get => healthBoost; set => healthBoost = value; }
    public float EnergyBoost { get => energyBoost; set => energyBoost = value; }
    public float AttackBoost { get => attackBoost; set => attackBoost = value; }


    [SerializeField] private float healthBoost;
    [SerializeField] private float energyBoost;
    [SerializeField] private float attackBoost;

    

    [SerializeField]
    [Header("The type of possibility this item offers.")]
    private PossibilityType type;
}

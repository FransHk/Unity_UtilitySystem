using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Text hpBoost, enBoost, atBoost;


    [SerializeField]
    [Header("The type of possibility this item offers.")]
    private PossibilityType type;


    private void Start()
    {
        if(hpBoost == null || enBoost == null || atBoost == null)
        {
            Debug.Log("Item.CS: No text components found, debug skipped.");
            return;
        }

        hpBoost.text = "HP: " + healthBoost.ToString();
        enBoost.text = "EN: " + energyBoost.ToString();
        atBoost.text = "AT: " + attackBoost.ToString();
    }
    
}

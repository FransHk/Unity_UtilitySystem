using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsibility:
/// Calculate utility
/// </summary>
public class Utility : MonoBehaviour
{   
    /// <summary>
    /// Returns utility for given state
    /// as a percentage
    /// </summary>
    public static float CalcUtility(PossibilityModel model, AgentBase agent)
    {
        if(model.Type == PossibilityType.APPLY_ITEM)
        {
            float resultingHealth, resultingEnergy, resultingAttack;
            float weightedHealth, weightedEnergy, weightedAttack;

            // Calculate our potential health, energy
            // and attack power levels after applying
            // an item to given agent
            resultingHealth = agent.AgentHealth + model.TargetItem.HealthBoost;
            if(resultingHealth > 100)
                resultingHealth = 100;

            resultingEnergy = agent.AgentEnergy + model.TargetItem.EnergyBoost;
            if(resultingEnergy > 100)
                resultingHealth = 100;

            resultingAttack = agent.AgentAttack + model.TargetItem.AttackBoost;
            if(resultingAttack > 100)
                resultingAttack = 100;

            weightedHealth = resultingHealth * agent.HealthWeight;
            weightedEnergy = resultingEnergy * agent.EnergyWeight;
            weightedAttack = resultingAttack * agent.AttackWeight;

            float utility = (weightedHealth + weightedEnergy + weightedAttack) / 3;

            Debug.Log("Agent: " + agent.name + " - Calculated utility of: " + utility + " % " 
             + "for an item");
        }

        return 0f;
    }


}

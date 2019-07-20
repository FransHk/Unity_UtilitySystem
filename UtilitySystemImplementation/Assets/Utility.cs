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
            float projectedHealth, projectedEnergy, projectedAttack;
            float healthIncrease, energyIncrease, attackIncrease;
            float weightedHealth, weightedEnergy, weightedAttack;

            // Calculate our potential health, energy
            // and attack power levels after applying
            // an item to given agent

            // Our projected health is the health
            // we should have after applying the item
            projectedHealth = agent.AgentHealth + model.TargetItem.HealthBoost;
            // Limit the health to 100%
            if(projectedHealth > 100)
                projectedHealth = 100;

            // Calculate the percentange of 
            // health gained from this item
            healthIncrease = projectedHealth - agent.AgentHealth;

            // Calculate the percentage of 
            // energy gained from this item
            projectedEnergy = agent.AgentEnergy + model.TargetItem.EnergyBoost;
            if(projectedEnergy > 100)
                projectedHealth = 100;

            energyIncrease = projectedEnergy - agent.AgentEnergy;

            // Calculate the percentage of 
            // attack power gained from item
            projectedAttack = agent.AgentAttack + model.TargetItem.AttackBoost;
            if(projectedAttack > 100)
                projectedAttack = 100;

            attackIncrease = projectedAttack - agent.AgentAttack;

            // Correct our gain with weights
            // that determine the agent's priorities
            weightedHealth = healthIncrease * agent.HealthWeight;
            weightedEnergy = energyIncrease * agent.EnergyWeight;
            weightedAttack = attackIncrease * agent.AttackWeight;

            float utility = (weightedHealth + weightedEnergy + weightedAttack) / 3;

            Debug.Log("Agent: " + agent.name + " - Calculated utility of: " + utility + " % " 
             + "for an item");

             return utility;
        }

        return 0f;
    }


}

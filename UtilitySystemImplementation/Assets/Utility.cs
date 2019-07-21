using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsibility:
/// Calculate utility
/// </summary>
public class Utility : MonoBehaviour
{   
    
    public const float ENERGY_COST_ATTACK = 10f;

    /// <summary>
    /// Returns utility for given state
    /// as a percentage
    /// </summary>
    public static float CalcUtility(PossibilityModel model, AgentBase agent)
    {
        
        // Calculate the utility percentage
        // of applying an item for this agent
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

            // Calculate the utility
            float utility = (weightedHealth + weightedEnergy + weightedAttack) / 3;

            // Debug the utility
            if(agent.EnableDebugs)
                Debug.Log("Agent: " + agent.name + " - Calculated utility of: " + utility + " % " 
                + "for an item");

             return utility;
        }
        
        // Calculate the utility percentage of 
        // an attack for this agent
        else if (model.Type == PossibilityType.ATTACK)
        {
            float projectedHealth, projectedEnergy;
            float enemyProjectedHealth;
            float projectedLoss, enemyProjectedLoss;
            float weightedDamageDealt, weightedHealthLoss;

            // Calculate the amount of health this
            // agent would have left in case it traded
            // damage with another agent
            projectedHealth = agent.AgentHealth - model.TargetAgent.AgentAttack;
            projectedEnergy = agent.AgentEnergy - ENERGY_COST_ATTACK;

            // If energy or health would fall below zero,
            // it has zero utility % for this agent 
            if(projectedEnergy <= 0 || projectedHealth < 0)
                return 0f;

            enemyProjectedHealth = model.TargetAgent.AgentHealth - agent.AgentAttack;
            
            projectedLoss = agent.AgentHealth - projectedHealth;
            enemyProjectedLoss = model.TargetAgent.AgentHealth - enemyProjectedHealth;

            weightedHealthLoss = (projectedLoss * agent.HealthWeight);
            weightedDamageDealt = (enemyProjectedLoss * agent.DealDamageWeight);

            float utility = (weightedHealthLoss + weightedDamageDealt) /2;

            // Debug the final utility outcome
            if(agent.EnableDebugs)
                Debug.Log("Agent: " + agent.name + " - Calculated utility of: " + utility + " % "
                + "for an attack");

            return utility;
        }

        // The possibility type is unknown and
        // therefore we return a utility % of
        // 0
        return 0f;
    }


}

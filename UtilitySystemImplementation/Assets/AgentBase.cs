using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Actions
{
    IDLE, MOVETO, ATTACK
}

public abstract class AgentBase : MonoBehaviour
{
    protected NavMeshAgent navMesh;
    protected float agentHealth;
    protected float agentEnergy;
    protected float agentAttack;

    // Start is called before the first frame update
    protected void Start()
    {
        navMesh = GetComponent<NavMeshAgent>(); 
    }

    /// <summary>
    /// Pick up an object in the game
    /// </summary>
    protected virtual IEnumerator PickupAction(GameObject item)
    {
        var dist = Vector3.Distance(transform.position, item.transform.position);
        navMesh.SetDestination(item.transform.position);

        yield return new WaitForSeconds(3f);
        
        if(dist < 2)
        {
            // reached object, pick it up/apply it
            ApplyItem(item);

        }
        else
        {
            StartCoroutine("PickupAction", item);
        }
        
    }

    /// <summary>
    /// Applies the effects of picking up
    /// an item to this agent
    /// </summary>
    /// <param name="item"></param>
    protected virtual void ApplyItem(GameObject item)
    {
        
        if(item.GetComponent<Item>() == null)
        {
            Debug.Log("Tried to apply an item that does not inherit from ItemBase.");
            return;
        }
        
        Item boostItem = GetComponent<Item>();
        
        agentHealth += boostItem.HealthBoost;
        agentEnergy += boostItem.EnergyBoost;
        agentAttack += boostItem.AttackBoost;

         
    }

    protected virtual void CancelNavigation()
    {
        navMesh.ResetPath();   
    }
}

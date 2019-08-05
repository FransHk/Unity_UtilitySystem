using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public abstract class AgentBase : MonoBehaviour, IPossibilityTarget
{
    private const float DISTANCE_BOUNDS = 12f;
    private const float ACTION_TICK_TIME = 1.5f;


    // All accessors of the
    // agent class
    public PossibilityType Type { get => type; set => type = value; }
    public float AgentHealth { get => agentHealth; set => agentHealth = value; }
    public float AgentEnergy { get => agentEnergy; set => agentEnergy = value; }
    public float AgentAttack { get => agentAttack; set => agentAttack = value; }
    public float HealthWeight { get => healthWeight; set => healthWeight = value; }
    public float EnergyWeight { get => energyWeight; set => energyWeight = value; }
    public float AttackWeight { get => attackWeight; set => attackWeight = value; }
    public float DealDamageWeight { get => dealDamageWeight; set => dealDamageWeight = value; }
    public bool EnableDebugs { get => enableDebugs; set => enableDebugs = value; }

    protected NavMeshAgent navMesh;
    [SerializeField] private float agentHealth = 100;
    [SerializeField] private float agentEnergy = 100;
    [SerializeField] private float agentAttack = 20;
    [SerializeField] private bool enableDebugs;

    // Weights that determine the 
    // agent's behaviour
    [Range(0, 1)]
    [SerializeField] private float healthWeight;
    [Range(0, 1)]
    [SerializeField] private float energyWeight;
    [Range(0, 1)]
    [SerializeField] private float attackWeight;
    [Range(0,1)]
    [SerializeField] private float dealDamageWeight;


    // Component references
    protected AgentSenses agentSenses;
    protected AgentDebug agentDebug;
    private Utility agentUtility;

    // This agent itself is also a 
    // attack possibility for other 
    // agents, this variable defines
    // that
    private PossibilityType type = PossibilityType.ATTACK;
   
    
    protected void Awake()
    {
        try
        {
            navMesh = GetComponent<NavMeshAgent>();
            agentSenses = GetComponent<AgentSenses>();
        }
        catch
        {
            throw new Exception("Agent " + gameObject.name + " is missing one or multiple components.");
        }
    
    }
    
    private void Start() 
    {
        StartCoroutine(CheckPossibilities());
    }
    private IEnumerator CheckPossibilities()
    {
        yield return new WaitForSeconds(ACTION_TICK_TIME);
        try
        {
            

            EvaluateUtility();
            StartCoroutine(CheckPossibilities());
        }
        catch
        {
            StartCoroutine(CheckPossibilities());
        }
 
    }

    protected virtual void EvaluateUtility()
    {   
        // Variables that keep that of
        // the highest utility action
        float highestUtility = 0;
        PossibilityModel highestModel = null;
        GameObject highestObj = null;

        // Iterate through all possibilities
        // in range of this agent
        foreach(GameObject obj in agentSenses.RadarRangeList)
        {
            // Create a possibility model
            // of that detected object
            PossibilityModel model = GetPossibilityModel(obj);
            
            // Calculate the utility %
            // of that given possibility
            float currentUtility = Utility.CalcUtility(model, this);

            // Compare and overwrite
            // if more suitable than
            // previous utility
            if(currentUtility > highestUtility)
            {
                highestUtility = currentUtility;
                highestModel = model;
                highestObj = obj;
            }
            
        }

        // After iterating, take highest
        // model and execute the corresponding
        // action
        if(highestObj != null)
        {
            if(highestModel.Type == PossibilityType.APPLY_ITEM)
            {
                StartCoroutine(PickupAction(highestObj));
            }
            else if(highestModel.Type == PossibilityType.ATTACK)
            {
                StartCoroutine(AttackAction(highestObj));
            }
        }
            
    }

    protected virtual IEnumerator AttackAction(GameObject target)
    {   
        // Navigate the agent to the 
        // attack target
        if(target == null)
            yield break;

        var dist = Vector3.Distance(transform.position, target.transform.position);

        navMesh.SetDestination(target.transform.position);

        yield return new WaitForSeconds(ACTION_TICK_TIME);

        // Check if we reached target,  
        if (dist < DISTANCE_BOUNDS)
        {
            Debug.Log("Agent reached target to attack.");

            // reached object, pick it up/apply it
            Attack(target);
        }
        else
        {
            StartCoroutine("AttackAction", target);
        }
    }

    /// <summary>
    /// Pick up an object in the game
    /// </summary>
    protected virtual IEnumerator PickupAction(GameObject item)
    {
        if(item == null)
            yield break;

        //Debug.Log("Agent: navigates to " + item.name + " to perform a pick-up.");
        var dist = Vector3.Distance(transform.position, item.transform.position);

        navMesh.SetDestination(item.transform.position);

        yield return new WaitForSeconds(ACTION_TICK_TIME);
        
        if(dist < DISTANCE_BOUNDS)
        {
            if(enableDebugs)
                Debug.Log("Agent reached item to pick up, attempting to pick it up and apply it..");

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
    /// <param name="itemObject"></param>
    protected virtual void ApplyItem(GameObject itemObject)
    {
        if(itemObject == null)
            return;
        
        Item boostItem = itemObject.GetComponent<Item>();
        
        agentHealth += boostItem.HealthBoost;
        agentEnergy += boostItem.EnergyBoost;
        agentAttack += boostItem.AttackBoost;

        // Clamp values to their maximum
        // of 100 each
        if(agentHealth > 100)
            agentHealth = 100;
        if(agentEnergy > 100)
            agentEnergy = 100;
        if(agentAttack > 100)
            agentAttack = 100;
        
        // Remove the applied item from
        // possibility list
        agentSenses.RadarRangeList.Remove(itemObject);
        
        // Destroy the item
        Destroy(itemObject);

        // End navigation
        this.CancelNavigation();
        
        if(EnableDebugs)
            Debug.Log("Agent successfully applied item: " + itemObject.name + " to itself.");
    }

    // Attack a valid agent base 
    // target
    protected virtual void Attack(GameObject target)
    {
        // Attempt to attack, this sometimes fails
        // if the target gets destroyed right before 
        // taking damage
        try
        {
            if (target.GetComponent<AgentBase>() != null)
            {
                target.GetComponent<AgentBase>().TakeDamage(agentAttack);
            }
        }
        catch
        {
           // Debug.Log("Warning: Agent tried to attack a target that was already destroyed..");
        }
       
    }

    // Cancel agent navigation by
    // resetting its path
    protected virtual void CancelNavigation()
    {
        navMesh.SetDestination(Vector3.zero); 
    }

    /// <summary>
    /// Convert an object that implements IPossbilityTarget
    /// to a possibility model that is usable by the
    /// Utiltiy calculation static method
    /// </summary>
    PossibilityModel GetPossibilityModel(GameObject obj)
    {
        if(obj == null)
            return null;   

        // The interface held by every possible
        // target for an action
        IPossibilityTarget target = obj.GetComponent<IPossibilityTarget>();
        PossibilityModel model;

        if (target == null)
            return null;

        // Create an item possibility
        if (target.Type == PossibilityType.APPLY_ITEM)
        {
            Item targetItem = obj.GetComponent<Item>();
            Vector3 targetPos = obj.transform.position;

            // Use the default constructor to create
            // a new possibility model for this item
            model = new PossibilityModel(target.Type, targetItem, targetPos);
        }

        // Create an attack possibility
        else if (target.Type == PossibilityType.ATTACK)
        {
            Vector3 targetPos = obj.transform.position;
            AgentBase targetAgent = obj.GetComponent<AgentBase>();

            // Use the overload constructor to create
            // a new possbility model for this enemy agent
            model = new PossibilityModel(target.Type, targetPos, targetAgent);

            //Debug.Log("Created a new attack possibility for agent: " + obj.name);
        }
        else
        {
            // Unknown target, abort
            if(enableDebugs)
                Debug.Log("Uknow target model found by agent..");

            return null;
        }

        return model;
    }

    /// <summary>
    /// The agent takes a certain
    /// amount of damage
    /// </summary>
    /// <param name="dmg"></param>
    public void TakeDamage(float dmg)
    {   
        AgentHealth -= dmg;
        if(AgentHealth <= 0)
        {
            Debug.Log("An agent was destroyed by taking fatal damage..");
            navMesh.enabled = false;
            Destroy(this.gameObject);
            
        }
    }

    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public abstract class AgentBase : MonoBehaviour, IPossibilityTarget
{
    private const float DISTANCE_BOUNDS = 12f;
    private const float AGENT_TICKRATE = 0.25f;
    private const float ACTION_DELAY = 1.5f;

    private const float ENERGY_REGEN_SCALE = 1f;


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
    protected bool actionReady = true;
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
            agentDebug = GetComponent<AgentDebug>();
        }
        catch
        {
            throw new Exception("Agent " + gameObject.name + " is missing one or multiple components.");
        }
    
    }
    
    private void Start() 
    {
        StartCoroutine(AgentTick());
    }

    /// <summary>
    /// The tick of this agent,
    /// executes all behaviour with
    /// a given tickrate delay.false
    /// 
    /// Lowering the delay will increase
    /// stress on CPU but will improve reaction
    /// time
    /// </summary>
    /// <returns></returns>
    private IEnumerator AgentTick()
    {

        // Agent recovers an amount of 1 energy
        // per second * the regen scale for
        // this agent 
        if (agentEnergy < 100)
            agentEnergy += AGENT_TICKRATE * ENERGY_REGEN_SCALE;

        // Updates the visual debug
        // parameters above the agent
        // on a canvas
        UpdateDebug();

        yield return new WaitForSeconds(AGENT_TICKRATE);

        try
        {
            EvaluateUtility();
            StartCoroutine(AgentTick());
        }
        catch
        {
            StartCoroutine(AgentTick());
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
            PossibilityModel model = Utility.GetPossibilityModel(obj);
            
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
            agentDebug.UpdateUtility(highestModel.Type.ToString(), (int)highestUtility); 
            if(highestModel.Type == PossibilityType.APPLY_ITEM)
            {
                StartCoroutine(PickupAction(highestObj));
            }
            else if(highestModel.Type == PossibilityType.ATTACK)
            {
                StartCoroutine(AttackAction(highestObj));
            }
        }
        else
        {
            agentDebug.UpdateUtility("Idle..", 0);
        }
            
    }

    protected virtual IEnumerator AttackAction(GameObject target)
    {   
        // Navigate the agent to the 
        // attack target
        if(target == null)
            yield break;

        var dist = Vector3.Distance(transform.position, target.transform.position);

        // Direct the navmesh towards
        // the attack target
        navMesh.SetDestination(target.transform.position);

        yield return new WaitForSeconds(AGENT_TICKRATE);

        // Check if we reached target,  
        if (dist < DISTANCE_BOUNDS)
        {
            if(enableDebugs)
                Debug.Log("Agent reached target to attack.");

            // Agent is in range and ready to attack,
            // perform the follow up
            if(actionReady)
            {
                agentEnergy -= Utility.ENERGY_COST_ATTACK;
                AttackFollowUpAction(target);
            }
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

        // Direct the navmesh towards the 
        // item to be applied
        
        try
        {
            if(navMesh.isOnNavMesh)
                navMesh.SetDestination(item.transform.position);
        }
        catch
        {
            //Debug.Log("Warning: Navmesh called on an empty agent.");
        }
      

        yield return new WaitForSeconds(AGENT_TICKRATE);
        
        // Check if the agent is within range
        // of the item to be applied
        if(dist < DISTANCE_BOUNDS)
        {
            if(enableDebugs)
                Debug.Log("Agent reached item to pick up, attempting to pick it up and apply it..");

            if(actionReady)
            {
                ApplyItemAction(item);
                actionReady = false;
                StartCoroutine(ResetActionCooldown());
            }

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
    protected virtual void ApplyItemAction(GameObject itemObject)
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
        this.CancelNavigationAction();
        
        if(EnableDebugs)
            Debug.Log("Agent successfully applied item: " + itemObject.name + " to itself.");
    }

    // Attack a valid agent base 
    // target
    protected virtual void AttackFollowUpAction(GameObject target)
    {
        // Attempt to attack, this sometimes fails
        // if the target gets destroyed right before 
        // taking damage
        try
        {
            if (target.GetComponent<AgentBase>() != null)
            {
                actionReady = false;
                target.GetComponent<AgentBase>().TakeDamageAction(agentAttack);

                // Takes the attack off cooldown
                // after a variable cooldown time
                // using a yield
                StartCoroutine(ResetActionCooldown());
            }
        }
        catch
        {
           // Debug.Log("Warning: Agent tried to attack a target that was already destroyed..");
        }
       
    }

    // Cancel agent navigation by
    // resetting its path
    protected virtual void CancelNavigationAction()
    {
        navMesh.SetDestination(Vector3.zero); 
    }


    /// <summary>
    /// The agent takes a certain
    /// amount of damage
    /// </summary>
    /// <param name="dmg"></param>
    public void TakeDamageAction(float dmg)
    {   
        agentDebug.FlashAgentColor(Color.red);

        AgentHealth -= dmg;
        if(AgentHealth <= 0)
        {
            Debug.Log("An agent was destroyed by taking fatal damage..");
            navMesh.enabled = false;
            Destroy(this.gameObject);    
        }    
    }

    private void UpdateDebug()
    {
        agentDebug.UpdateHP((int)this.agentHealth);
        agentDebug.UpdateEN((int)this.agentEnergy);
        agentDebug.UpdateAT((int)this.agentAttack);
    }

    private IEnumerator ResetActionCooldown()
    {
        yield return new WaitForSeconds(ACTION_DELAY);
        actionReady = true;
    }
    
}

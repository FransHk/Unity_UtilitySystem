using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public abstract class AgentBase : MonoBehaviour, IPossibilityTarget
{
    private const float DISTANCE_BOUNDS = 8f;
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
    private PossibilityType type = PossibilityType.ATTACK;
    private Utility agentUtility;
    



    // Start is called before the first frame update
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
        yield return new WaitForSeconds(2f);
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
        float highestUtility = 0;
        PossibilityModel highestModel = null;
        GameObject highestObj = null;

        foreach(GameObject obj in agentSenses.RadarRangeList)
        {
            
            PossibilityModel model = GetPossibilityModel(obj);
            
            float currentUtility = Utility.CalcUtility(model, this);

            if(currentUtility > highestUtility)
            {
                highestUtility = currentUtility;
                highestModel = model;
                highestObj = obj;
            }
            
        }

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
        if(target == null)
            yield break;

        //Debug.Log("Agent: navigates to " + item.name + " to perform a pick-up.");
        var dist = Vector3.Distance(transform.position, target.transform.position);

        navMesh.SetDestination(target.transform.position);

        yield return new WaitForSeconds(3f);

        if (dist < DISTANCE_BOUNDS)
        {
            Debug.Log("Agent reached item to pick up, attempting to pick it up and apply it..");

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

        yield return new WaitForSeconds(3f);
        
        if(dist < DISTANCE_BOUNDS)
        {
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

        if(agentHealth > 100)
            agentHealth = 100;
        if(agentEnergy > 100)
            agentEnergy = 100;
        if(agentAttack > 100)
            agentAttack = 100;
        
        agentSenses.RadarRangeList.Remove(itemObject);
        Destroy(itemObject);
        this.CancelNavigation();

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
        //navMesh.ResetPath();   
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
            Debug.Log("Uknow target model found by agent..");
            return null;
        }

        return model;
    }

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

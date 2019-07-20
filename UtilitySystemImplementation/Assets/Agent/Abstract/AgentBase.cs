using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public abstract class AgentBase : MonoBehaviour, IPossibilityTarget
{
    // All accessors of the
    // agent class
    public PossibilityType Type { get => type; set => type = value; }
    public float AgentHealth { get => agentHealth; set => agentHealth = value; }
    public float AgentEnergy { get => agentEnergy; set => agentEnergy = value; }
    public float AgentAttack { get => agentAttack; set => agentAttack = value; }
    public float HealthWeight { get => healthWeight; set => healthWeight = value; }
    public float EnergyWeight { get => energyWeight; set => energyWeight = value; }
    public float AttackWeight { get => attackWeight; set => attackWeight = value; }

    protected NavMeshAgent navMesh;
    [SerializeField] private float agentHealth = 100;
    [SerializeField] private float agentEnergy = 100;
    [SerializeField] private float agentAttack = 20;

    // Weights that determine the 
    // agent's behaviour
    [Range(0, 1)]
    [SerializeField] private float healthWeight;
    [Range(0, 1)]
    [SerializeField] private float energyWeight;
    [Range(0, 1)]
    [SerializeField] private float attackWeight;

    // Component references
    protected AgentSenses agentSenses;
    private PossibilityType type;
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
        EvaluateUtility();
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(CheckPossibilities());
    }

    protected virtual void EvaluateUtility()
    {
        float highestUtility = 0;
        PossibilityModel highestModel;

        foreach(GameObject obj in agentSenses.RadarRangeList)
        {
            
            PossibilityModel model = GetPossibilityModel(obj);
            float currentUtility = Utility.CalcUtility(model, this);

            if(currentUtility > highestUtility)
            {
                highestUtility = currentUtility;
                highestModel = model;
            }
            
            
        }
            
    }

    PossibilityModel GetPossibilityModel(GameObject obj)
    {
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

                //Debug.Log("Created a new item possibility for object: " + obj.name);
            }

            // Create an attack possibility
            else if (target.Type == PossibilityType.ATTACK)
            {
                Vector3 targetPos = obj.transform.position;
                AgentBase targetAgent = obj.GetComponent<AgentBase>();

                // Use the overload constructor to create
                // a new possbility model for this enemy agent
                model = new PossibilityModel(target.Type, targetPos, targetAgent);

                Debug.Log("Created a new attack possibility for agent: " + obj.name);
            }
            else
            {
                // Unknown target, abort
                Debug.Log("Uknow target model found by agent..");
                return null;
            }

            return model;
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
            Debug.Log("Agent reched item to pick up, attemtping to pick it up and apply it..");
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

        Debug.Log("Agent successfully applied item: " + item.name + " to itself.");
    }

    protected virtual void AttackPlayer(AgentInstance targetAgent)
    {
        
    }

    protected virtual void CancelNavigation()
    {
        navMesh.ResetPath();   
    }
}

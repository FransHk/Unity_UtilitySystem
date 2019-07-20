using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PossibilityType
{
    APPLY_ITEM, NAVIGATE, ATTACK
}

public class PossibilityModel : MonoBehaviour
{
    public PossibilityType Type { get; private set; }
    public Item TargetItem { get; private set; }
    public Vector3 TargetPos { get; private set; }
    public AgentBase TargetAgent;

    // Constructor for item possibility
    public PossibilityModel(PossibilityType type, Item targetItem, Vector3 targetPos)
    {
        this.Type = type;

        if(targetItem != null)
            this.TargetItem = targetItem;
        if(targetPos != null)
            this.TargetPos = targetPos;
  
    }

    // Constructor overload for attack possibillity
    public PossibilityModel(PossibilityType type, Vector3 targetPos, AgentBase targetAgent)
    {
        this.Type = type;

        if (targetPos != null)
            this.TargetPos = targetPos;
        if (targetAgent != null)
            this.TargetAgent = targetAgent;
    }
    

}

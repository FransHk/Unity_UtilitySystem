using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Actions
{
    IDLE, MOVETO, ATTACK
}
public class Agent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Returns utility for given state
    /// as a percentage
    /// </summary>
    float CalcUtility(Actions action)
    {
        float utility = 0f;



        return utility;
    }
}

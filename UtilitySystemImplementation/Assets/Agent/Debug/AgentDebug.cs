using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentDebug : MonoBehaviour
{
    [SerializeField]
    private Text hpText, enText, atText, utText;
    private AgentInstance agent;

    private void Start() 
    {
        try
        {
            agent = GetComponent<AgentInstance>();
        }
        catch
        {
            throw new Exception("AgentDebug: Error while fetching required components for debugging.");
        }
    }


    /// <summary>
    /// The methods below are responsible for
    /// updating the HP, Enery, Attack
    /// and utility debug texts that float
    /// above the agent in world space
    /// </summary>
    
    public void UpdateHP(int amt)
    {
        hpText.text = "HP: " + amt;
    }

    public void UpdateEN(int amt)
    {
        enText.text = "EN: " + amt;
    }

    public void UpdateAT(int amt)
    {
        atText.text = "AT: " + amt;
    }

    public void UpdateUtility(string action, int amt) 
    {
        utText.text = "Action: " + action + "with " + amt + "%";
    }

}

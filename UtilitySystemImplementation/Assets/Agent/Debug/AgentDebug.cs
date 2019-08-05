using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles all debug
/// information that is displayed
/// above the agents on the field
/// </summary>
public class AgentDebug : MonoBehaviour
{
    [SerializeField]
    private Text hpText, enText, atText, utText;
    private int currentHP, currentEN, currentAT;
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
        if(currentHP == amt)
            return;

        hpText.text = "HP: " + amt;
        hpText.color = Color.yellow;

        currentHP = amt;
        StartCoroutine(ResetIndicator(hpText));
    }

    public void UpdateEN(int amt)
    {
        if(currentEN == amt)
            return;

        enText.text = "EN: " + amt;
        enText.color = Color.yellow;

        currentEN = amt;

        StartCoroutine(ResetIndicator(enText));
    }

    public void UpdateAT(int amt)
    {
        if(currentAT == amt)
            return;

        atText.text = "AT: " + amt;
        atText.color = Color.yellow;

        currentAT = amt;


        StartCoroutine(ResetIndicator(atText));
    }

    public void UpdateUtility(string action, int amt) 
    {
        utText.text = "Action: " + action + " with " + amt + "%";
        utText.color = Color.yellow;

        StartCoroutine(ResetIndicator(utText));
    }

    public void FlashAgentColor(Color color)
    {
        StartCoroutine(FlashCoroutine(color));
    }

    private IEnumerator FlashCoroutine(Color color)
    {
        Material mat = GetComponent<Renderer>().material;
        Color originalColor = mat.color;

        mat.color = color;

        yield return new WaitForSeconds(0.5f);
        mat.color = originalColor;
    }

    private IEnumerator ResetIndicator(Text textToReset)
    {
        yield return new WaitForSeconds(2f);
        textToReset.color = Color.white;
    }

}

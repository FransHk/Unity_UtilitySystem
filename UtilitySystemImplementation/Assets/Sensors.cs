using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensors : MonoBehaviour
{   
    private const float INTERACTABLE_RADIUS = 1f;
    private const float RADAR_RADIUS = 6f;
    public List<GameObject> InInteractableRange = new List<GameObject>();
    public List<GameObject> OnAgentRadar = new List<GameObject>();



    private void Start()
    {
        //UpdateInteractableRange();
        UpdateOnRadarObjs();
    }

    /// <summary>
    /// Update list with objects that are 
    /// within interactable range for agent
    /// </summary>
    public void UpdateInteractableRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, INTERACTABLE_RADIUS);
        foreach(Collider c in hitColliders)
        {
            if(ValidObj(c.gameObject, InInteractableRange))
            {
                InInteractableRange.Add(c.gameObject);
            }
        }
    }

    /// <summary>
    /// Updatel ist iwht objecs that are
    /// within sight of the agent
    /// </summary>
    public void UpdateOnRadarObjs()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, RADAR_RADIUS);
        
        foreach (Collider c in hitColliders)
        {
            if (ValidObj(c.gameObject, OnAgentRadar))
                OnAgentRadar.Add(c.gameObject);
        }
    }

    private bool ValidObj(GameObject obj, List<GameObject> targetList)
    {
        Debug.Log("Valid check started for obj: " + obj.name);   
        if(obj == this.gameObject)
            return false;
        if(targetList.Contains(obj))
            return false;
        else
        {
            Debug.Log("Valid check for obj: " + obj.name + " returned true. ");
            return true;
        }
    }
}

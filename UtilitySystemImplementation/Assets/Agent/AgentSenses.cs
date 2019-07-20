using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSenses : MonoBehaviour
{   
    private const float INTERACTABLE_RADIUS = 1f;
    private const float RADAR_RADIUS = 10f;
    private const float REFRESH_DELAY = 2f;
    public List<GameObject> InteractableRangeList = new List<GameObject>();
    public List<GameObject> RadarRangeList = new List<GameObject>();



    private void Start()
    {
        StartCoroutine(UpdateVision());
    }

    private IEnumerator UpdateVision()
    {
        this.UpdateInteractableRange();
        this.UpdateOnRadarObjs();

        yield return new WaitForSeconds(REFRESH_DELAY);

        StartCoroutine(UpdateVision());
    }

    /// <summary>
    /// Update list with objects that are 
    /// within interactable range for agent
    /// </summary>
    private void UpdateInteractableRange()
    {
        InteractableRangeList.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, INTERACTABLE_RADIUS);
        foreach(Collider c in hitColliders)
        {
            if(ValidObj(c.gameObject, InteractableRangeList))
            {
                InteractableRangeList.Add(c.gameObject);
            }
        }
    }

    /// <summary>
    /// Updatel ist iwht objecs that are
    /// within sight of the agent
    /// </summary>
    private void UpdateOnRadarObjs()
    {
        RadarRangeList.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, RADAR_RADIUS);
        
        foreach (Collider c in hitColliders)
        {
            if (ValidObj(c.gameObject, RadarRangeList))
                RadarRangeList.Add(c.gameObject);
        }
    }

    /// <summary>
    /// Determines if the object the agent
    /// detects is a valid possbility for 
    /// utility calculation.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetList"></param>
    /// <returns></returns>
    private bool ValidObj(GameObject obj, List<GameObject> targetList)
    {
        // If the object is not a valid
        // possibility target, false
        if(obj.GetComponent<IPossibilityTarget>() == null)
            return false;

        // If we detected ourself, false
        if(obj == this.gameObject)
            return false;

        // If this object is already in the list
        // we also return false
        if(targetList.Contains(obj))
            return false;
        else
        {
            // Passed all checks, so the 
            // obj is suited to be added 
            // to the list
            return true;
        }
    }
}

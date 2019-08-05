using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private int spawnAmount;
    [SerializeField]
    private float spawnRadius = 40f;
    [SerializeField] private float heightOffset = 2.6f;

    [SerializeField]
    private GameObject prefabToSpawn;

    [SerializeField]
    private float maxHealth, maxEnergy, maxAttack;
    
    // Start is called before the first frame update
    void Start()
    {
        this.SpawnItems();
    }

    private void SpawnItems()
    {
        for(int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
            Vector3 correctedPos = new Vector3(randomPos.x, heightOffset, randomPos.z);

            GameObject prefab = Instantiate(prefabToSpawn, correctedPos, Quaternion.identity);  
            
            // Parent the new item to 
            // this object
            prefab.transform.parent = this.gameObject.transform;

            // Set up the item and determine
            // the boost to health, energy 
            // and attack
            Item item = prefab.GetComponent<Item>();
            item.HealthBoost = (int)Random.Range(1, maxHealth);
            item.EnergyBoost = (int)Random.Range(1, maxEnergy);
            item.AttackBoost = (int)Random.Range(1, maxAttack);
        }
    }
}

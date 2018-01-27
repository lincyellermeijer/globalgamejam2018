using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    public GameObject[] spawnLocation;
    private GameObject[] spawnedObjects;

    private void Start()
    {
        spawnedObjects = new GameObject[5];
    }

    public void SpawnObject()
    {
        List<int> usedIndices = new List<int>();

        for (int i = 0; i < spawnedObjects.Length; i++)
        {
            int randomIndex = Random.Range(0, spawnLocation.Length);
            while (usedIndices.Contains(randomIndex)) randomIndex = Random.Range(0, spawnLocation.Length);
            GameObject item = Instantiate(Resources.Load("Prefabs/Collectible"), spawnLocation[randomIndex].transform.position, spawnLocation[randomIndex].transform.rotation) as GameObject;
            spawnedObjects[i] = item;
            usedIndices.Add(randomIndex);
        }
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] public GameObject objectToSpawn;
    [SerializeField] int spawnAmount;
    //[SerializeField] float spawnRate;

    public List<Transform> points = new List<Transform>();

    //int spawnCount;
    //float spawnTimer;

    bool startSpawning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            points.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (startSpawning)
        //{
        //    spawnTimer += Time.deltaTime;

        //    if (spawnCount < spawnAmount && spawnTimer >= spawnRate)
        //    {
        //        spawn();
        //    }
        //}
    }

    public void spawn()
    {
        //spawnTimer = 0;
        //spawnCount++;
        for (int i = 0; i < spawnAmount; i++)
        {
            Instantiate(objectToSpawn, points[Random.Range(0, points.Count - 1)].transform.position, Quaternion.identity);
        }
    }

    public void spawn(int amount)
    {
        spawnAmount = amount;
        for (int i = 0; i < spawnAmount; i++)
        {
            Instantiate(objectToSpawn, points[Random.Range(0, points.Count - 1)].transform.position, Quaternion.identity);
        }
    }

    public void spawnAssign(GameObject animal, int amount = 1)
    {
        if (objectToSpawn != animal)
        {
            objectToSpawn = animal;
        }

        if (spawnAmount != amount)
        {
            spawnAmount = amount;
        }

        spawn();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        startSpawning = true;
    //    }
    //}
}

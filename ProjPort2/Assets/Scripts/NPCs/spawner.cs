using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class spawner : MonoBehaviour
{
    [SerializeField] public GameObject objectToSpawn;
    [SerializeField] int spawnAmount;
    //[SerializeField] float spawnRate;
    [SerializeField] ParticleSystem enemySpawnEffect;

    public List<Transform> points = new List<Transform>();

    //int spawnCount;
    //float spawnTimer;

    bool startSpawning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        foreach (Transform child in gameObject.transform)
        {
            points.Add(child);
        }
    }
    void Start()
    {

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
            Vector3 point = points[Random.Range(0, points.Count - 1)].transform.position;
            StartCoroutine(spawnWithDelay(point));
        }
    }

    public void spawn(int amount)
    {
        spawnAmount = amount;
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 point = points[Random.Range(0, points.Count - 1)].transform.position;
            StartCoroutine(spawnWithDelay(point));
        }
    }

    public void spawnAssign(GameObject animal, int amount)
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

    private IEnumerator spawnWithDelay(Vector3 point)
    {
        Instantiate(enemySpawnEffect, point, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(objectToSpawn, point, Quaternion.identity);
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        startSpawning = true;
    //    }
    //}
}

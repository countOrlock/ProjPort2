using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    [SerializeField] GameObject smallGame1;
    [SerializeField] GameObject smallGame2;
    [SerializeField] GameObject mediumGame1;
    [SerializeField] GameObject mediumGame2;
    [SerializeField] GameObject mediumHostileGame;
    [SerializeField] GameObject bigGame;

    [SerializeField] int smallGame1SpawnLimit;
    [SerializeField] int smallGame2SpawnLimit;
    [SerializeField] int mediumGame1SpawnLimit;
    [SerializeField] int mediumGame2SpawnLimit;
    [SerializeField] int mediumHostileGameSpawnLimit;

    // List of all spawners
    GameObject[] spawners;

    // Buckets of spawners sorted by what they spawn
    List<List<spawner>> spawnerBuckets;

    void Awake()
    {
        // Finding all spawners
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        
        // Sorting all the buckets
        for (int i = 0; i < spawners.Length; i++)
        {
            spawner spawnerObject = spawners[i].GetComponent<spawner>();
            
            for (int j = 0; j < spawnerBuckets.Count; j++)
            {

                if (spawnerBuckets[i][0].objectToSpawn == spawnerObject.objectToSpawn)
                {
                    spawnerBuckets[i].Add(spawnerObject);
                }
                else
                {
                    spawnerBuckets.Add(new List<spawner> {spawnerObject});
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Spawning the initial animals
        Spawn(smallGame1, smallGame1SpawnLimit);
        Spawn(smallGame2, smallGame2SpawnLimit);
        Spawn(mediumGame1, mediumGame1SpawnLimit);
        Spawn(mediumGame2, mediumGame2SpawnLimit);
        Spawn(mediumHostileGame, mediumHostileGameSpawnLimit);
        Spawn(bigGame, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void Spawn(GameObject spawnObject, int numToSpawn)
    {
        if (!spawnerBuckets.Exists(x => x[0].objectToSpawn == spawnObject))
        {
            Debug.LogError("Cannot Spawn Object: NO MATCHING BUCKET FOUND");
            return;
        }
        List<spawner> spawners = spawnerBuckets.Find(x => x[0].objectToSpawn == spawnObject);
        
        for (int i = 0; i < numToSpawn; i++)
        {
            spawners[(i + 1) % spawners.Count].spawn();
            questManager.instance.UpdateQuest(spawnObject, 1);
        }
    }
}

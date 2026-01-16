using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

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
    List<List<spawner>> spawnerBuckets = new List<List<spawner>>();

    // Buckets of living NPCs sorted by NPC
    List<List<string>> livingNPCBuckets = new List<List<string>>();

    void Awake()
    {
        instance = this;

        // Finding all spawners
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        
        // Sorting all the buckets
        for (int i = 0; i < spawners.Length; i++)
        {
            spawner spawnerObject = spawners[i].GetComponent<spawner>();
            
            if (spawnerBuckets.Count == 0)
            {
                spawnerBuckets.Add(new List<spawner> { spawnerObject });
            }
            else
            {
                for (int j = 0; j < spawnerBuckets.Count; j++)
                {
                    if (spawnerBuckets[j][0].objectToSpawn == spawnerObject.objectToSpawn)
                    {
                        spawnerBuckets[j].Add(spawnerObject);
                    }
                    else
                    {
                        spawnerBuckets.Add(new List<spawner> { spawnerObject });
                        break;
                    }
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Spawning the initial animals
        Spawn(smallGame1,        smallGame1SpawnLimit);
        Spawn(smallGame2,        smallGame2SpawnLimit);
        Spawn(mediumGame1,       mediumGame1SpawnLimit);
        Spawn(mediumGame2,       mediumGame2SpawnLimit);
        Spawn(mediumHostileGame, mediumHostileGameSpawnLimit);
        Spawn(bigGame,           1);
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
            spawners[(i + spawners.Count) % spawners.Count].spawn();
        }

        UpdateNPCCount(spawnObject, numToSpawn);
    }

    public void UpdateNPCCount(GameObject NPC, int amount)
    {
        if (amount == 0)
        {
            return;
        }

        // Getting the model name (some recognizable element from the NPC prefab)
        bool isAnimal = NPC.GetComponent<animalAI>();

        string npcModelName = "-1";

        if (isAnimal)
        {
            npcModelName = NPC.GetComponent<animalAI>().model.ToString();
        }
        else if (!isAnimal)
        {
            npcModelName = NPC.GetComponent<enemyAI>().model.ToString();
        }

        // Sorting all the buckets based on the model name
        if (amount > 0)
        {
            for (int i = 0; i < amount; i++)
            {
                questManager.instance.UpdateQuest(NPC, 1);
                if (livingNPCBuckets.Count == 0)
                {
                    livingNPCBuckets.Add(new List<string> { npcModelName });
                }
                else
                {
                    for (int j = 0; j < livingNPCBuckets.Count; j++)
                    {
                        if (livingNPCBuckets[j][0] == npcModelName)
                        {
                            livingNPCBuckets[j].Add(npcModelName);
                        }
                        else
                        {
                            livingNPCBuckets.Add(new List<string> { npcModelName });
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i > amount; i--)
            {
                if (livingNPCBuckets.Count == 0)
                {
                    return;
                }
                else
                {
                    for (int j = 0; j < livingNPCBuckets.Count; j++)
                    {
                        if (livingNPCBuckets[j][0] == npcModelName)
                        {
                            livingNPCBuckets[j].Remove(npcModelName);
                            if (livingNPCBuckets[j].Count == 0)
                            {
                                livingNPCBuckets.RemoveAt(j);
                            }
                            questManager.instance.UpdateQuest(NPC, -1);
                            break;
                        }
                    }
                }
            }
        }

    }
}

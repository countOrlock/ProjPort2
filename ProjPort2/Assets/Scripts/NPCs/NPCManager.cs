using UnityEngine;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance;

    [Header("-----Animals-----")]
    [SerializeField] GameObject smallGame1;
    [SerializeField] GameObject smallGame2;
    [SerializeField] GameObject mediumGame1;
    [SerializeField] GameObject mediumGame2;
    [SerializeField] GameObject mediumGameHostile;
    [SerializeField] GameObject bigGame;
    [SerializeField] GameObject warden;
    [SerializeField] GameObject alien;

    [Header("-----Spawners-----")]
    [SerializeField] spawner smallGame1Spawner;
    [SerializeField] spawner smallGame2Spawner;
    [SerializeField] spawner mediumGame1Spawner;
    [SerializeField] spawner mediumGame2Spawner;
    [SerializeField] spawner mediumGameHostileSpawner;
    [SerializeField] spawner bigGameSpawner;
    [SerializeField] spawner wardenSpawner;
    [SerializeField] spawner alienSpawner;

    [Header("-----Min-----")]
    [SerializeField] int smallGame1SpawnMinimum;
    [SerializeField] int smallGame2SpawnMinimum;
    [SerializeField] int mediumGame1SpawnMinimum;
    [SerializeField] int mediumGame2SpawnMinimum;
    [SerializeField] int mediumGameHostileSpawnMinimum;
    [SerializeField] int bigGameSpawnMinimum;

    [Header("-----Max-----")]
    [SerializeField] int smallGame1SpawnLimit;
    [SerializeField] int smallGame2SpawnLimit;
    [SerializeField] int mediumGame1SpawnLimit;
    [SerializeField] int mediumGame2SpawnLimit;
    [SerializeField] int mediumGameHostileSpawnLimit;
    [SerializeField] int bigGameSpawnLimit;

    // List of all spawners
    GameObject[] spawners;

    // Buckets of spawners sorted by what they spawn
    //List<List<spawner>> spawnerBuckets = new List<List<spawner>>();

    // Buckets of living NPCs sorted by NPC
    List<List<string>> livingNPCBuckets = new List<List<string>>();

    void Awake()
    {
        instance = this;

        // Finding all spawners
        spawners = GameObject.FindGameObjectsWithTag("Spawner");


        // Sorting all the buckets
        //for (int i = 0; i < spawners.Length; i++)
        //{
        //    spawner spawnerObject = spawners[i].GetComponent<spawner>();
        //    string npcModelName = GetModelName(spawnerObject.objectToSpawn);

        //    if (spawnerBuckets.Count == 0)
        //    {
        //        spawnerBuckets.Add(new List<spawner> { spawnerObject });
        //    }
        //    else
        //    {
        //        for (int j = 0; j < spawnerBuckets.Count; j++)
        //        {
        //            if (GetModelName(spawnerBuckets[j][0].objectToSpawn) == GetModelName(spawnerObject.objectToSpawn))
        //            {
        //                spawnerBuckets[j].Add(spawnerObject);
        //            }
        //            else if (j == spawnerBuckets.Count - 1)
        //            {
        //                spawnerBuckets.Add(new List<spawner> { spawnerObject });
        //                break;
        //            }
        //        }
        //    }
        //}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Spawning the initial animals
        //Spawn(smallGame1,        smallGame1SpawnLimit);
        //Spawn(smallGame2,        smallGame2SpawnLimit);
        //Spawn(mediumGame1,       mediumGame1SpawnLimit);
        //Spawn(mediumGame2,       mediumGame2SpawnLimit); 
        //Spawn(mediumHostileGame, mediumHostileGameSpawnLimit);
        //Spawn(bigGame,           bigGameSpawnLimit);

        int random = Random.Range(smallGame1SpawnMinimum, smallGame1SpawnLimit);
        smallGame1Spawner.spawnAssign(smallGame1, random);
        UpdateNPCCount(smallGame1, random);

        random = Random.Range(smallGame2SpawnMinimum, smallGame2SpawnLimit);
        smallGame2Spawner.spawnAssign(smallGame2, random);
        UpdateNPCCount(smallGame2, random);

        random = Random.Range(mediumGame1SpawnMinimum, mediumGame1SpawnLimit);
        mediumGame1Spawner.spawnAssign(mediumGame1, random);
        UpdateNPCCount(mediumGame1, random);

        random = Random.Range(mediumGame2SpawnMinimum, mediumGame2SpawnLimit);
        mediumGame2Spawner.spawnAssign(mediumGame2, random);
        UpdateNPCCount(mediumGame2, random);

        random = Random.Range(mediumGameHostileSpawnMinimum, mediumGameHostileSpawnLimit);
        mediumGameHostileSpawner.spawnAssign(mediumGameHostile, random);
        UpdateNPCCount(mediumGameHostile, random);

        random = Random.Range(bigGameSpawnMinimum, bigGameSpawnLimit);
        bigGameSpawner.spawnAssign(bigGame, random);
        UpdateNPCCount(bigGame, random);

        wardenSpawner.spawnAssign(warden, 1);

        alienSpawner.spawnAssign(alien, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void Spawn(GameObject NPC, int numToSpawn)
    {
        string npcModelName = GetModelName(NPC);

        spawner spawn;

        if (npcModelName == smallGame1.GetComponent<animalAI>().model.ToString())
        {
            spawn = smallGame1Spawner;
        }
        else if (npcModelName == smallGame2.GetComponent<animalAI>().model.ToString())
        {
            spawn = smallGame2Spawner;
        }
        else if (npcModelName == mediumGame1.GetComponent<animalAI>().model.ToString())
        {

            spawn = mediumGame1Spawner;
        }
        else if (npcModelName == mediumGame2.GetComponent<animalAI>().model.ToString())
        {
            spawn = mediumGame2Spawner;
        }
        else if (npcModelName == mediumGameHostile.GetComponent<animalAI>().model.ToString())
        {
            spawn = mediumGameHostileSpawner;
        }
        else
        {
            spawn = bigGameSpawner;
        }

        spawn.spawn(numToSpawn);

        UpdateNPCCount(NPC, numToSpawn);
    }

    public void UpdateNPCCount(GameObject NPC, int amount)
    {
        if (amount == 0)
        {
            return;
        }

        // Getting the model name (some recognizable element from the NPC prefab)
        string npcModelName = GetModelName(NPC);

        // Sorting/adjusting the buckets based on the NPC model name
        // For a positive adjustment: (an NPC was spawned)
        if (amount > 0)
        {
            for (int i = 0; i < amount; i++)
            {
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
                            break;
                        }
                        else if (j == livingNPCBuckets.Count - 1)
                        {
                            livingNPCBuckets.Add(new List<string> { npcModelName });
                            break;
                        }
                    }
                }
            }
        }
        // For a negative adjustment: (an NPC died)
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
                            // Ensuring the minimum NPC count is still kept
                            int min = CheckNPCMin(NPC);
                            if (livingNPCBuckets[j].Count - 1 < min)
                            {
                                Spawn(NPC, (min - (livingNPCBuckets[j].Count - 1)));
                            }

                            livingNPCBuckets[j].Remove(npcModelName);

                            // Removing the bucket if it's empty
                            if (livingNPCBuckets[j].Count == 0)
                            {
                                livingNPCBuckets.RemoveAt(j);
                            }

                            // Updating any quests using this NPC
                            questManager.instance.UpdateCurrentQuest(NPC, 1);
                            break;
                        }
                    }
                }
            }
        }
    }

    string GetModelName(GameObject NPC)
    {
        bool isAnimal = NPC.GetComponent<animalAI>();

        if (isAnimal)
        {
            return NPC.GetComponent<animalAI>().model.ToString();
        }
        else if (!isAnimal)
        {
            return NPC.GetComponent<enemyAI>().model.ToString();
        }

        return "-1";
    }

    int CheckNPCMin(GameObject NPC)
    {
        string npcModelName = GetModelName(NPC);

        int min = -1;

        // Determining the Spawn minimum
        if (npcModelName == smallGame1.GetComponent<animalAI>().model.ToString())
        {
            min = smallGame1SpawnMinimum;
        }
        else if (npcModelName == smallGame2.GetComponent<animalAI>().model.ToString())
        {
            min = smallGame2SpawnMinimum;
        }
        else if (npcModelName == mediumGame1.GetComponent<animalAI>().model.ToString())
        {

            min = mediumGame1SpawnMinimum;
        }
        else if (npcModelName == mediumGame2.GetComponent<animalAI>().model.ToString())
        {

            min = mediumGame2SpawnMinimum;
        }
        else if (npcModelName == mediumGameHostile.GetComponent<animalAI>().model.ToString())
        {

            min = mediumGameHostileSpawnMinimum;
        }
        else if (npcModelName == bigGame.GetComponent<animalAI>().model.ToString())
        {
            min = bigGameSpawnMinimum;
        }

        return min;
    }

    public void AlienSpawnEvent()
    {
        alienSpawner.spawn(1);
    }

    public void WardenDeathEvent()
    {
        wardenSpawner.spawn(1);
    }
}

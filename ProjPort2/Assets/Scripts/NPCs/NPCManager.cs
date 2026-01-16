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

    [SerializeField] int smallGame1SpawnMinimum;
    [SerializeField] int smallGame2SpawnMinimum;
    [SerializeField] int mediumGame1SpawnMinimum;
    [SerializeField] int mediumGame2SpawnMinimum;
    [SerializeField] int mediumHostileGameSpawnMinimum;
    [SerializeField] int bigGameSpawnMinimum;

    [SerializeField] int smallGame1SpawnLimit;
    [SerializeField] int smallGame2SpawnLimit;
    [SerializeField] int mediumGame1SpawnLimit;
    [SerializeField] int mediumGame2SpawnLimit;
    [SerializeField] int mediumHostileGameSpawnLimit;
    [SerializeField] int bigGameSpawnLimit;

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
            string npcModelName = GetModelName(spawnerObject.objectToSpawn);

            if (spawnerBuckets.Count == 0)
            {
                spawnerBuckets.Add(new List<spawner> { spawnerObject });
            }
            else
            {
                for (int j = 0; j < spawnerBuckets.Count; j++)
                {
                    if (GetModelName(spawnerBuckets[j][0].objectToSpawn) == GetModelName(spawnerObject.objectToSpawn))
                    {
                        spawnerBuckets[j].Add(spawnerObject);
                    }
                    else if (j == spawnerBuckets.Count - 1)
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
        Spawn(bigGame,           bigGameSpawnLimit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void Spawn(GameObject NPC, int numToSpawn)
    {
        string npcModelName = GetModelName(NPC);

        if (!spawnerBuckets.Exists(x => GetModelName(x[0].objectToSpawn) == npcModelName))
        {
            Debug.LogError("Cannot Spawn Object: NO MATCHING BUCKET FOUND");
            return;
        }
        List<spawner> spawners = spawnerBuckets.Find(x => GetModelName(x[0].objectToSpawn) == npcModelName);
        
        for (int i = 0; i < numToSpawn; i++)
        {
            spawners[(i + spawners.Count) % spawners.Count].spawn();
        }

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
        else if (npcModelName == mediumHostileGame.GetComponent<animalAI>().model.ToString())
        {

            min = mediumHostileGameSpawnMinimum;
        }
        else if (npcModelName == bigGame.GetComponent<animalAI>().model.ToString())
        {
            min = bigGameSpawnMinimum;
        }

        return min;
    }
}

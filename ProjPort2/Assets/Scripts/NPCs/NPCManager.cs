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

    GameObject[] spawners;

    void Awake()
    {
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

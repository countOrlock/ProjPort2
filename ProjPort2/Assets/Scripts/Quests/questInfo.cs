using UnityEngine;

[CreateAssetMenu]

public class questInfo : ScriptableObject
{
    public GameObject animal;
    public GameObject questAnimalSpawn;

    public int reward;
    public string questName;
    public string questObjective;
    [Range(1, 30)] public int numOfAnimalsToHunt;
    public Status questStatus;
    public enum Status
    {
        Not_Accepted, // 0
        In_Progress_Slot1, // 1
        In_Progress_Slot2, // 2
        Completed // 3
    }
}

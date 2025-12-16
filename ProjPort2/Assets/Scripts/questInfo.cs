using UnityEngine;

[CreateAssetMenu]

public class questInfo : ScriptableObject
{
    public GameObject questObject;

    public string questName;
    public string questObjective;
    [Range(0, 10)] public int itemsForQuest; // lowest is 0 for now for testing purposes till the quest system if fully set up
    [Range (1, 3)] public int questStatus;
    public enum questID
    {
        Completed = 1,
        In_Progress, // 2
        Not_Accepted // 3
    }
}

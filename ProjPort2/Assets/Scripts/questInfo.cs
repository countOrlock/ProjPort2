using UnityEngine;

[CreateAssetMenu]

public class questInfo : ScriptableObject
{
    public GameObject questObject;

    public string questName;
    public string questObjective;
    public int itemsForQuest;
    [Range (0, 2)]public int questStatus;
    public enum questID
    {
        Completed, // 0
        In_Progress, // 1
        Not_Accepted // 2
    }
}

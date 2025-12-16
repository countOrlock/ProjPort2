using UnityEngine;

[CreateAssetMenu]

public class questInfo : ScriptableObject
{
    public GameObject questObject;

    public string questName;
    public string questObjective;
    [Range(0, 10)] public int itemsForQuest; // lowest is 0 for now for testing purposes till the quest system if fully set up
    [Range (0, 2)] public int questStatus;
}

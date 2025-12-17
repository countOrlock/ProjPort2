using UnityEngine;

[CreateAssetMenu]

public class questInfo : ScriptableObject
{
    public GameObject questObject;
    public GameObject animal;

    public int reward;
    public string questName;
    public string questObjective;
}

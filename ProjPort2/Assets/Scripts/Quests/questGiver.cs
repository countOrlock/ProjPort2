using UnityEngine;

public class questGiver : MonoBehaviour, IGiveQuest
{
    [SerializeField] questInfo quest;
    GameObject[] spawners;

    private void Start()
    {
        
    }

    public questInfo giveQuest()
    {
        if (quest != null)
        {
            return quest;
        }
        else
        {
            return new questInfo();
        }
    }

    public int giveReward()
    {
        return quest.reward;
    }
}

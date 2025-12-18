using UnityEngine;

public class questGiver : MonoBehaviour, IGiveQuest
{
    [SerializeField] questInfo quest;
    public GameObject[] spawners;

    private void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
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
        gameManager.instance.currQuestLoc = null;
        return quest.reward;
    }
}

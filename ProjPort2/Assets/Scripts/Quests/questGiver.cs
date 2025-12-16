using UnityEngine;

public class questGiver : MonoBehaviour, IGiveQuest
{
    [SerializeField] questInfo quest;

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
}

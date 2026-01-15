using UnityEngine;
using System.Collections.Generic;

public class questGiver : MonoBehaviour
{
    [SerializeField] List<questInfo> availableQuestList = new List<questInfo>();

    public void giveQuest(questInfo quest)
    {
        // move the quest from the (to-do quest list in the Quest Manager to an active quest slot)
        // remove the quest from the quest giver's available quest list
    }
}

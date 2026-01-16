using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

public class questManager : MonoBehaviour
{
    public static questManager instance;

    [SerializeField] List<questInfo> unavailableQuests;
    [SerializeField] List<questInfo> availableQuests;
    public questInfo activeQuest1;
    public questInfo activeQuest2;
    public List<questInfo> completeQuests = new List<questInfo>();

    private int quest1Target;
    private int quest1Current;
    private int quest2Target;
    private int quest2Current;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quest1Current = 0;
        quest2Current = 0;

        if (activeQuest1)
        {
            quest1Target = activeQuest1.numOfAnimalsToHunt;
            gameManager.instance.updateActiveQuest1(activeQuest1.questName, quest1Current, quest1Target);
        }
        else
        {
            quest1Target = -1;
        }

        if (activeQuest2)
        {
            quest2Target = activeQuest2.numOfAnimalsToHunt;
            gameManager.instance.updateActiveQuest2(activeQuest2.questName, quest2Current, quest2Target);
        }
        else
        {
            quest2Target = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateQuest(GameObject animal, int amount)
    {
        if (activeQuest1 != null || activeQuest2 != null)
        {
            if (activeQuest1.animal.GetComponent<animalAI>().model.ToString() == animal.GetComponent<animalAI>().model.ToString())
            {
                quest1Current += amount;
                gameManager.instance.updateActiveQuest1(activeQuest1.questName, quest1Current, quest1Target);
            }
            else if (activeQuest2.animal.GetComponent<animalAI>().model.ToString() == animal.GetComponent<animalAI>().model.ToString())
            {
                quest2Current += amount;
                gameManager.instance.updateActiveQuest2(activeQuest2.questName, quest2Current, quest2Target);
            }
            else
            {
                return;
            }
        }

        CheckQuestStatus();
    }

    void CheckQuestStatus()
    {
        if (quest1Current == quest1Target)
        {
            CompleteQuest(1);
        }

        if (quest2Current == quest2Target)
        {
            CompleteQuest(2);
        }
        }

    void CompleteQuest(int activeQuest)
    {
        GameObject currentQuestAnimal;
        if (activeQuest != 1 && activeQuest != 2)
        {
            Debug.LogError("Invalid Quest, Cannot Complete");
            return;
        }
        else if (activeQuest == 1)
        {
            currentQuestAnimal = activeQuest1.animal;
            completeQuests.Add(activeQuest1);
            gameManager.instance.player.GetComponent<playerController>().Gold += activeQuest1.reward;
            gameManager.instance.updateGameGoal(activeQuest1.reward);
            activeQuest1 = null;
        }
        else
        {
            currentQuestAnimal = activeQuest2.animal;
            completeQuests.Add(activeQuest2);
            gameManager.instance.player.GetComponent<playerController>().Gold += activeQuest2.reward;
            gameManager.instance.updateGameGoal(activeQuest2.reward);
            activeQuest2 = null;
        }

        if (availableQuests.Count != 0)
        {
            GiveNewQuest(availableQuests[0]);
            availableQuests.Remove(availableQuests[0]);
        }

        questInfo nextLevelQuest = FindNextLevelQuest(currentQuestAnimal);

        if (nextLevelQuest)
        {
            availableQuests.Add(nextLevelQuest);
            unavailableQuests.Remove(nextLevelQuest);
        }
    }

    questInfo FindNextLevelQuest(GameObject questAnimal)
    {
        for (int i = 0; i < unavailableQuests.Count; i++)
        {
            if (unavailableQuests[i].animal == questAnimal)
            {
                return unavailableQuests[i];
            }
        }
        return null;
    }

    void GiveNewQuest(questInfo newQuest)
    {
        if (!activeQuest1)
        {
            activeQuest1  = newQuest;
            quest1Target  = activeQuest1.numOfAnimalsToHunt;
            quest1Current = 0;
            gameManager.instance.updateActiveQuest1(newQuest.questName, 0, newQuest.numOfAnimalsToHunt);
        }
        else if (!activeQuest2)
        {
            activeQuest2  = newQuest;
            quest2Target  = activeQuest2.numOfAnimalsToHunt;
            quest2Current = 0;
            gameManager.instance.updateActiveQuest2(newQuest.questName, 0, newQuest.numOfAnimalsToHunt);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

public class questManager : MonoBehaviour
{
    public static questManager instance;

    [Header("-----General-----")]
    [SerializeField] public List<questInfo> unavailableQuests;
    [SerializeField] public List<questInfo> availableQuests;
    public questInfo activeQuest1;
    public questInfo activeQuest2;
    public List<questInfo> completeQuests = new List<questInfo>();

    private int quest1Target;
    private int quest1Current;
    private int quest2Target;
    private int quest2Current;

    [Header("-----Day Info-----")]
    float dayTimeInSeconds;
    float dayTimerInSeconds;


    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // For Day Initialization
        dayTimeInSeconds = DayManager.instance.currentDay.dayLengthInMinutes * 60;

        // For Quest Initialization
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

        gameManager.instance.updateAvailableQuests();
    }

    // Update is called once per frame
    void Update()
    {
        dayTimerInSeconds += Time.deltaTime;
        CheckTimeLeft();
        CheckRentPaid();
    }

    void CheckTimeLeft()
    {
        gameManager.instance.updateDayTime(dayTimeInSeconds - dayTimerInSeconds);
        if (dayTimerInSeconds > dayTimeInSeconds)
        {
            dayTimerInSeconds = 0;
            gameManager.instance.youLose();
        }
    }

    public void UpdateCurrentQuest(GameObject animal, int amount)
    {
        if (activeQuest1 != null &&
            activeQuest1.animal != null && 
            activeQuest1.animal.GetComponent<animalAI>().model.ToString() == animal.GetComponent<animalAI>().model.ToString())
        {
            quest1Current += amount;
            gameManager.instance.updateActiveQuest1(activeQuest1.questName, quest1Current, quest1Target);
        }
        else if (activeQuest2 != null && 
            activeQuest2.animal != null && 
            activeQuest2.animal.GetComponent<animalAI>().model.ToString() == animal.GetComponent<animalAI>().model.ToString())
        {
            quest2Current += amount;
            gameManager.instance.updateActiveQuest2(activeQuest2.questName, quest2Current, quest2Target);
        }
        else
        {
            return;
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

    public void CheckRentPaid()
    {
        if (gameManager.instance.player.GetComponent<playerController>().Gold >= DayManager.instance.currentDay.rentAmountDue)
        {
            gameManager.instance.youWin();
        }
    }

    void CompleteQuest(int activeQuest)
    {
        questInfo completedQuest;
        if (activeQuest != 1 && activeQuest != 2)
        {
            Debug.LogError("Invalid Quest, Cannot Complete");
            return;
        }
        else if (activeQuest == 1)
        {
            completedQuest = activeQuest1;
            completeQuests.Add(activeQuest1);
            gameManager.instance.player.GetComponent<playerController>().Gold += activeQuest1.reward;
            DayManager.instance.UpdateGoldEarned(activeQuest1.reward);
            gameManager.instance.updateGameGoal(activeQuest1.reward);
            activeQuest1 = null;
        }
        else
        {
            completedQuest = activeQuest2;
            completeQuests.Add(activeQuest2);
            gameManager.instance.player.GetComponent<playerController>().Gold += activeQuest2.reward;
            DayManager.instance.UpdateGoldEarned(activeQuest2.reward);
            gameManager.instance.updateGameGoal(activeQuest2.reward);
            activeQuest2 = null;
        }

        if (availableQuests.Count != 0)
        {
            GiveNewQuest(availableQuests[0], completedQuest);
        }

        DayManager.instance.UpdateQuests();
        DayManager.instance.UpdateQuestsCompletedDuringDay(completedQuest);
    }

    questInfo FindNextLevelQuest(questInfo quest)
    {
        for (int i = 0; i < unavailableQuests.Count; i++)
        {
            if (unavailableQuests[i].animal == quest.animal)
            {
                return unavailableQuests[i];
            }
        }
        return null;
    }

    public bool GiveNewQuest(questInfo newQuest, questInfo completedQuest = null)
    {
        if (newQuest == null || newQuest.animal == null)
        {
            if (completedQuest)
            {
                if (activeQuest1 == null)
                {
                    activeQuest1 = newQuest;
                    gameManager.instance.updateActiveQuest1(newQuest.questName, 0, newQuest.numOfAnimalsToHunt);
                    quest1Target  = 0;
                    quest1Current = 0;
                }
                if (activeQuest2 == null)
                {
                    activeQuest2 = newQuest; 
                    gameManager.instance.updateActiveQuest2(newQuest.questName, 0, newQuest.numOfAnimalsToHunt);
                    quest2Target  = 0;
                    quest2Current = 0;
                }
            }

            DayManager.instance.UpdateQuests();
            return false;
        }

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
        else
        {
            DayManager.instance.UpdateQuests();
            return false;
        }

            availableQuests.Remove(newQuest);
        if (completedQuest)
        {
            MoveUnavailableQuestToAvailableQuest(completedQuest);
        }
        else
        {
            MoveUnavailableQuestToAvailableQuest();
        }

        DayManager.instance.UpdateQuests();

        return true;
    }

    void MoveUnavailableQuestToAvailableQuest(questInfo completedQuest = null)
    {
        if (completedQuest)
        {
            questInfo nextLevelQuest = FindNextLevelQuest(completedQuest);

            if (nextLevelQuest)
            {
                availableQuests.Add(nextLevelQuest);
                unavailableQuests.Remove(nextLevelQuest);
            }
            else if (completeQuests.Exists(x => x.animal == completedQuest))
            {
                questInfo newQuest = completeQuests.Find(x => x.animal == completedQuest);
                availableQuests.Add(newQuest);
            }
            else
            {
                questInfo newQuest = new questInfo();
                availableQuests.Add(newQuest);
            }
        }
        else if (unavailableQuests.Count != 0)
        {
            availableQuests.Add(unavailableQuests[0]);
            unavailableQuests.Remove(unavailableQuests[0]);
        }
        else
        {
            questInfo newQuest = new questInfo();
            availableQuests.Add(newQuest);
        }

        gameManager.instance.updateAvailableQuests();
    }

    public void ResetQuestsToStartOfDay()
    {
        unavailableQuests = DayManager.instance.currentDayInfoAtStartOfDay.unavailableQuests;
        availableQuests   = DayManager.instance.currentDayInfoAtStartOfDay.availableQuests;
        activeQuest1      = DayManager.instance.currentDayInfoAtStartOfDay.activeQuest1;
        activeQuest2      = DayManager.instance.currentDayInfoAtStartOfDay.activeQuest2;
        completeQuests    = DayManager.instance.currentDayInfoAtStartOfDay.TotalCompletedQuests;
    }
}

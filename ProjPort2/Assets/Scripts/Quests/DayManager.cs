using UnityEngine;
using System.Collections.Generic;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;

    [SerializeField] public DayInfo firstDay;
    public List<DayInfo> previousDays;
    public DayInfo currentDay;
    [SerializeField] public DayInfo currentDayInfoAtStartOfDay;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (firstDay == null)
        {
            firstDay = new DayInfo();
        }

        currentDay = firstDay;
        UpdateQuests();
        CopyDay(currentDay, currentDayInfoAtStartOfDay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CopyDay(DayInfo dayToCopy, DayInfo toRecieve)
    {
        toRecieve.dayNumber                = dayToCopy.dayNumber;
        toRecieve.dayLengthInMinutes       = dayToCopy.dayLengthInMinutes;
        toRecieve.rentAmountDue            = dayToCopy.rentAmountDue;
        toRecieve.unavailableQuests        = dayToCopy.unavailableQuests;
        toRecieve.availableQuests          = dayToCopy.availableQuests;
        toRecieve.activeQuest1             = dayToCopy.activeQuest1;
        toRecieve.activeQuest2             = dayToCopy.activeQuest2;
        toRecieve.TotalCompletedQuests     = dayToCopy.TotalCompletedQuests;
        toRecieve.questsCompletedDuringDay = dayToCopy.questsCompletedDuringDay;
        toRecieve.goldEarned               = dayToCopy.goldEarned;
        toRecieve.NPCsKilled               = dayToCopy.NPCsKilled;
        toRecieve.playerGunList            = dayToCopy.playerGunList;
    }

    DayInfo NewDayBasedOnPreviousDay()
    {
        DayInfo newDay = new DayInfo(currentDay.dayNumber + 1, currentDay.dayLengthInMinutes, currentDay.rentAmountDue + 20,
            currentDay.unavailableQuests, currentDay.availableQuests, currentDay.TotalCompletedQuests, new List<questInfo>(),
            currentDay.activeQuest1, currentDay.activeQuest2, 0, new List<List<string>>(), currentDay.playerGunList);
        return newDay;
    }

    public void UpdateQuests()
    {
        currentDay.unavailableQuests          = questManager.instance.unavailableQuests;
        currentDay.availableQuests            = questManager.instance.availableQuests;
        currentDay.activeQuest1               = questManager.instance.activeQuest1;
        currentDay.activeQuest2               = questManager.instance.activeQuest2;
        currentDay.TotalCompletedQuests       = questManager.instance.completeQuests;
    }

    public void UpdateQuestsCompletedDuringDay(questInfo completedQuest)
    {
        currentDay.questsCompletedDuringDay.Add(completedQuest);
    }

    public void UpdateGoldEarned(int amount)
    {
        currentDay.goldEarned += amount;
    }

    public void UpdateNPCsKilled(GameObject NPC)
    {
        string npcModelName = NPCManager.instance.GetModelName(NPC);

        if (currentDay.NPCsKilled.Exists(x => x[0] == npcModelName))
        {
            currentDay.NPCsKilled.Find(x => x[0] == npcModelName).Add(npcModelName);
        }

    }

    public void UpdatePlayerGunList(gunStats gun)
    {
        currentDay.playerGunList.Add(gun);
    }

    //public void UpdateItems(Item newItem)
    //{

    //}

    public void ResetDay()
    {
        CopyDay(currentDay, currentDayInfoAtStartOfDay);
        gameManager.instance.ResetPlayerToStartOfDay();
        questManager.instance.ResetQuestsToStartOfDay();
    }


}

using UnityEngine;
using System.Collections.Generic;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;

    public List<DayInfo> previousDays;
    public DayInfo currentDay;

    public int dayNumber;
    public float dayLengthInMinutes;
    public int rentAmountDue;
    public List<questInfo> unavailableQuests;
    public List<questInfo> availableQuests;
    public questInfo activeQuest1;
    public questInfo activeQuest2;
    public List<questInfo> TotalCompletedQuests;
    public List<questInfo> questsCompletedDuringDay;
    public int goldEarned;
    public List<List<string>> NPCsKilled;
    public List<gunStats> playerGunList;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetDayToCurrentDay();
        UpdateQuests();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetDayToCurrentDay()
    {
        dayNumber                = currentDay.dayNumber;
        dayLengthInMinutes       = currentDay.dayLengthInMinutes;
        rentAmountDue            = currentDay.rentAmountDue;
        unavailableQuests        = currentDay.unavailableQuests;
        availableQuests          = currentDay.availableQuests;
        activeQuest1             = currentDay.activeQuest1;
        activeQuest2             = currentDay.activeQuest2;
        TotalCompletedQuests     = currentDay.TotalCompletedQuests;
        questsCompletedDuringDay = currentDay.questsCompletedDuringDay;
        goldEarned               = currentDay.goldEarned;
        NPCsKilled               = currentDay.NPCsKilled;
        playerGunList            = currentDay.playerGunList;
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
        unavailableQuests          = questManager.instance.unavailableQuests;
        availableQuests            = questManager.instance.availableQuests;
        activeQuest1               = questManager.instance.activeQuest1;
        activeQuest2               = questManager.instance.activeQuest2;
        TotalCompletedQuests       = questManager.instance.completeQuests;
    }

    public void UpdateGoldEarned(int amount)
    {
        goldEarned += amount;
    }

    public void UpdateNPCsKilled(GameObject NPC)
    {
        string npcModelName = NPCManager.instance.GetModelName(NPC);

        if (NPCsKilled.Exists(x => x[0] == npcModelName))
        {
            NPCsKilled.Find(x => x[0] == npcModelName).Add(npcModelName);
        }

    }

    public void UpdatePlayerGunList(gunStats gun)
    {
        playerGunList = gameManager.instance.player.GetComponent<playerController>().gunList;
    }

    //public void UpdateItems(Item newItem)
    //{

    //}

    public void ResetDay()
    {
        SetDayToCurrentDay();
        gameManager.instance.ResetPlayerToStartOfDay();
        questManager.instance.ResetQuestsToStartOfDay();
    }


}

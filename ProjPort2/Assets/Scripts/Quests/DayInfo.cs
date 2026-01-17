using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

[CreateAssetMenu]

public class DayInfo : ScriptableObject

{
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

    // Track what items the Player bought during that day (besides guns)
    // public List<Item> itemsBought;

    public DayInfo(int _dayNumber = -1, float _dayLengthInMinutes = -1, int _rentAmountDue = -1, 
        List<questInfo> _unavailableQuests = null, List<questInfo> _availableQuests = null, List<questInfo> _TotalCompletedQuests = null,
        List<questInfo> _questsCompletedDuringDay = null, questInfo _activeQuest1 = null, questInfo _activeQuest2 = null,
        int _goldEarned = -1, List<List<string>> _NPCsKilled = null, List<gunStats> _playerGunList = null)
    { 
        dayNumber                   = _dayNumber;
        dayLengthInMinutes          = _dayLengthInMinutes;
        rentAmountDue               = _rentAmountDue;
        unavailableQuests           = _unavailableQuests;
        availableQuests             = _availableQuests;
        activeQuest1                = _activeQuest1;
        activeQuest2                = _activeQuest2;
        TotalCompletedQuests        = _TotalCompletedQuests;
        questsCompletedDuringDay    = _questsCompletedDuringDay;
        goldEarned                  = _goldEarned;
        NPCsKilled                  = _NPCsKilled;
        playerGunList               = _playerGunList;
    }

        public DayInfo(DayInfo dayToCopy)
    { 
        dayNumber                   = dayToCopy.dayNumber;
        dayLengthInMinutes          = dayToCopy.dayLengthInMinutes;
        rentAmountDue               = dayToCopy.rentAmountDue;
        unavailableQuests           = dayToCopy.unavailableQuests;
        availableQuests             = dayToCopy.availableQuests;
        activeQuest1                = dayToCopy.activeQuest1;
        activeQuest2                = dayToCopy.activeQuest2;
        TotalCompletedQuests        = dayToCopy.TotalCompletedQuests;
        questsCompletedDuringDay    = dayToCopy.questsCompletedDuringDay;
        goldEarned                  = dayToCopy.goldEarned;
        NPCsKilled                  = dayToCopy.NPCsKilled;
        playerGunList               = dayToCopy.playerGunList;
    }
}

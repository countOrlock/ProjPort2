using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class questManager : MonoBehaviour
{
    public static questManager instance;

    [SerializeField] List<List<questInfo>> toDoQuests;
    [SerializeField] List<questInfo> availableQuests;
    public questInfo activeQuest1;
    public questInfo activeQuest2;
    List<questInfo> completeQuests;

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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateQuest(GameObject killedAnimal)
    {
        if (activeQuest1 != null || activeQuest2 != null)
        {
            if (activeQuest1.animal.name == killedAnimal.name)
            {
                quest1Current++;
            }
            else if (activeQuest1.animal.name == killedAnimal.name)
            {
                quest2Current++;
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
        if (activeQuest != 1 || activeQuest != 2)
        {
            Debug.LogError("Invalid Quest, Cannot Complete");
            return;
        }
        else if (activeQuest == 1)
        {
            completeQuests.Add(activeQuest1);
            activeQuest1 = null;
        }
        else
        {
            completeQuests.Add(activeQuest2);
            activeQuest2 = null;
        }

        GiveNewQuest();
        addToAvailableQuests();
        RemoveFromQuestList();
    }

    void addToAvailableQuests()
    {

    }

    void RemoveFromQuestList()
    {

    }

    void GiveNewQuest()
    {

    }

    void UpdateQuestList()
    {

    }
}

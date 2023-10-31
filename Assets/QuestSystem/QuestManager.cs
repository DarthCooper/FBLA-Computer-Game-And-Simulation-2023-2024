using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;

    private int currentPlayerLevel = 0;

    private void Awake()
    {
        questMap = CreateQuestMap();
    }

    private void OnEnable()
    {
        Manager.Instance.questEvents.onStartQuest += StartQuest;
        Manager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
        Manager.Instance.questEvents.onFinishQuest += FinishQuest;
    }

    private void Start()
    {
        foreach (Quest quest in questMap.Values)
        {
            Manager.Instance.questEvents.QuestStateChange(quest, QuestState.REQUIREMENTS_NOT_MET);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        Manager.Instance.questEvents.QuestStateChange(quest, state);
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;
        //if(currentPlayerLevel < quest.info.levelRequirement)
        //{
        //    meetsRequirements = false;
        //}

        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if(GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    private void Update()
    {
        foreach (Quest quest in questMap.Values)
        {
            if(quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void OnDisable()
    {
        Manager.Instance.questEvents.onStartQuest -= StartQuest;
        Manager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        Manager.Instance.questEvents.onFinishQuest -= FinishQuest;
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstatiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
        Debug.Log("Start Quest: " +  id);
    }

    private void AdvanceQuest(string id)
    {
        print(id);
        Quest quest = GetQuestById(id);
        quest.MoveToNextStep();
        if(quest.CurrentStepExists())
        {
            quest.InstatiateCurrentQuestStep(this.transform);
        }else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
        Debug.Log("Advance Quest: " + id);
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
        Debug.Log("Finish Quest" + id);
    }

    public void ClaimRewards(Quest quest)
    {
        //interact with inventory
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach(QuestInfoSO questInfo in allQuests)
        {
            if(idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if(quest == null)
        {
            Debug.LogError("ID not found in Quest Map: " + id);
        }
        return quest;
    }
}

using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour, IDataPersistence
{
    public static QuestManager instance;

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    private Dictionary<string, Quest> questMap;

    public List<QuestStep> steps;

    private int currentPlayerLevel = 0;

    private GameObject player;

    bool needToLoad = false;

    public static bool isServer = false;

    private void Awake()
    {
        questMap = CreateQuestMap();
        instance = this;
    }

    private void OnEnable()
    {
        Manager.Instance.questEvents.onStartQuest += StartQuest;
        Manager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
        Manager.Instance.questEvents.onFinishQuest += FinishQuest;

        Manager.Instance.questEvents.onQuestStepStateChange += QuestStepStateChange;
    }

    private void OnDisable()
    {
        Manager.Instance.questEvents.onStartQuest -= StartQuest;
        Manager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        Manager.Instance.questEvents.onFinishQuest -= FinishQuest;

        Manager.Instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;
    }

    private void Start()
    {
        CheckToSpawnQuest(QuestState.REQUIREMENTS_NOT_MET);
        InvokeRepeating(nameof(CheckLoadData), 0.1f, 10f);
        InvokeRepeating(nameof(SetQuestsTransform), 0f, 5f);
        player = GameObject.Find("LocalGamePlayer");
        isServer = player.GetComponent<NetworkIdentity>().isServer;
    }

    public void CheckToSpawnQuest(QuestState state)
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstatiateCurrentQuestStep(this.transform);
            }
            else if(quest.state == QuestState.CAN_FINISH)
            {
                Journal.Instance.AddQuest(quest);
            }
            ChangeQuestState(quest.info.id, state);
        }
    }

    public void CheckToSpawnSpecificQuest(Quest quest, QuestState state)
    {
        if (quest.state == QuestState.IN_PROGRESS)
        {
            quest.InstatiateCurrentQuestStep(this.transform);
        }
        else if (quest.state == QuestState.CAN_FINISH)
        {
            Journal.Instance.AddQuest(quest);
        }
        ChangeQuestState(quest.info.id, state);
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

    public void SetQuestsTransform()
    {
        GameObject[] questSteps = GameObject.FindGameObjectsWithTag("QuestIdentifier");
        foreach (var quest in questSteps){
            if(quest.transform.parent != this.transform)
            {
                quest.transform.SetParent(this.transform);
            }
        }
    }

    public void AddQuestStep(QuestStep step)
    {
        steps.Add(step);
    }

    public QuestStep getQuestStep(string id, string questStepId)
    {
        foreach (var step in steps)
        {
            if(step.questId.Equals(id) && step.questStepID.Equals(questStepId))
            {
                return step;
            }
        }
        return null;
    }

    public bool DuplicateQuestStep(QuestStep questStep)
    {
        bool alreadySpawned = false;
        foreach (var step in steps)
        {
            if(step.questId == questStep.questId)
            {
                if (step.questStepID == questStep.questStepID)
                {
                    alreadySpawned = true;
                }
            }
        }
        return alreadySpawned;
    }

    public void CheckLoadData()
    {
        if (needToLoad && data != null)
        {
            questMap = CreateQuestMap();
            foreach (var quest in questMap.Values)
            {
                CheckToSpawnSpecificQuest(quest, quest.state);
            }
        }
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstatiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
        Debug.Log("Start Quest: " +  id);
    }

    public void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);
        print(quest.info.id +" : " + quest.currentQuestStepIndex);
        quest.MoveToNextStep();
        print(quest.info.id + " : " + quest.currentQuestStepIndex);
        if(quest.CurrentStepExists())
        {
            quest.InstatiateCurrentQuestStep(this.transform);
        }else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
        Debug.Log("Advance Quest: " + id);
    }

    public void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
        Debug.Log("Finish Quest" + id);
        Journal.Instance.RemoveQuest(quest);
    }

    public void ClaimRewards(Quest quest)
    {
        //for items only
        foreach (var item in quest.info.itemRewards)
        {
            Inventory.Instance.AddItem(item);
        }
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        if(data!= null)
        {
            foreach(QuestInfoSO questInfo in allQuests)
            {
                if(idToQuestMap.ContainsKey(questInfo.id))
                {
                    Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
                }
                idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
            }
            needToLoad = false;
        }
        else
        {
            needToLoad = true;
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

    GameData data;
    public void LoadData(GameData data)
    {
        this.data = data;
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        if(data == null) { return new Quest(questInfo); }
        try
        {
            if(data.questData.ContainsKey(questInfo.id) && loadQuestState) 
            {
                string serializedData = data.questData[questInfo.id];
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                if(questData.state == QuestState.REQUIREMENTS_NOT_MET)
                {
                    return quest = new Quest(questInfo);
                }
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }else
            {
                quest = new Quest(questInfo);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
    }

    public void SaveData(ref GameData data)
    {
        data.questData.Clear();
        foreach (Quest quest in questMap.Values)
        {
            try
            {
                QuestData questData = quest.GetQuestData();
                print(quest.info.id + " : " + questData.questStepIndex);
                string serializedData = JsonUtility.ToJson(questData);
                data.questData.Add(quest.info.id, serializedData);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
            }
        }
    }

}

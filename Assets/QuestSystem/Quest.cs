using Mirror;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestInfoSO info;
    public QuestState state;
    public int currentQuestStepIndex;
    private QuestStepState[] questStepStates;

    public List<QuestStep> questSteps = new List<QuestStep>();

    public Quest(QuestInfoSO questInfo)
    {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
        this.questStepStates = new QuestStepState[info.questStepPrefabs.Length];
        this.info.previouslyLoaded = false;

        Debug.Log("New Save: " + info.id + " : " + currentQuestStepIndex);

        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
    {
        this.info = questInfo;
        this.state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;
        this.info.previouslyLoaded = true;

        Debug.Log("Loading: " + info.id + " : " + currentQuestStepIndex);

        if (this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning("Quest Step Prefabs and Quest Step States are " + "of different lengths. This indicates something changed " + "with the QuestInfo and the saved data is now out of sync. " + "Reset your data - as this might cause issues. QuestId: " + this.info.id);
        }
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return(currentQuestStepIndex < info.questStepPrefabs.Length);
    }

    public QuestStep GetCurrentStep()
    {
        if(currentQuestStepIndex >= questSteps.Count)
        {
            return questSteps[questSteps.Count - 1];
        }
        return questSteps[currentQuestStepIndex];
    }

    public void InstatiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null && !QuestManager.instance.DuplicateQuestStep(questStepPrefab.GetComponent<QuestStep>()))
        {
            QuestStep questStep = Object.Instantiate<GameObject>(questStepPrefab, parentTransform).GetComponent<QuestStep>();
            questStep.InitializeQuestStep(info.id, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state, this);
            questSteps.Add(questStep);
            QuestManager.instance.AddQuestStep(questStep);
            Journal.Instance.AddQuest(this);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if(CurrentStepExists())
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
        }else
        {
            Debug.LogWarning("Tried to get quest step prefab, but stepINdex was out of range indicating that" + "there's no current step: QuestId=" + info.id + ", stepIndex=" + currentQuestStepIndex);
        }
        return questStepPrefab;
    }

    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
    {
        if(stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].state = questStepState.state;
        }else
        {
            Debug.LogWarning("Tried to access quest step data, but stepINdex was out of range: " + "Quest Id = " + info.id + ", Step Index = " + stepIndex);
        }
    }

    public QuestData GetQuestData()
    {
        return new QuestData(state, currentQuestStepIndex, questStepStates);
    }
}

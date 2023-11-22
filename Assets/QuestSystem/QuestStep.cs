using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class QuestStep : NetworkBehaviour
{
    private bool isFinished = false;
    public string questStepID;
    public string questId;
    private int stepIndex = 0;

    public Quest quest;

    public string progress;
    public string maxProgress;

    public void InitializeQuestStep(string questId, int stepIndex, string questStepState, Quest quest)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        this.quest = quest;
        if(questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }

    public void FinishQuestStep()
    {
        if(!isFinished)
        {
            isFinished = true;
            Manager.Instance.questEvents.AdvanceQuest(questId);
            Destroy(gameObject);
            Journal.Instance.RemoveQuest(quest);
        }
    }

    protected void ChangeState(string newState)
    {
        Manager.Instance.questEvents.QuestStepStateChange(questId, stepIndex, new QuestStepState(newState));
    }

    protected abstract void SetQuestStepState(string state);
}

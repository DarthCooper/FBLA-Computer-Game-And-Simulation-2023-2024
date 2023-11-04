using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class QuestStep : NetworkBehaviour
{
    private bool isFinished = false;
    public string questId;
    private int stepIndex = 0;

    public Quest quest;

    public string progress;

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

    protected void FinishQuestStep()
    {
        if(!isFinished)
        {
            isFinished = true;
            Manager.Instance.questEvents.AdvanceQuest(questId);
            Destroy(gameObject);
        }
    }

    protected void ChangeState(string newState)
    {
        Manager.Instance.questEvents.QuestStepStateChange(questId, stepIndex, new QuestStepState(newState));
    }

    protected abstract void SetQuestStepState(string state);
}
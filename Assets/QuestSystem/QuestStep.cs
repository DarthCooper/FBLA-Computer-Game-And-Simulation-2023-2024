using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    public string questId;
    private int stepIndex = 0;

    public void InitializeQuestStep(string questId, int stepIndex, string questStepState)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
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

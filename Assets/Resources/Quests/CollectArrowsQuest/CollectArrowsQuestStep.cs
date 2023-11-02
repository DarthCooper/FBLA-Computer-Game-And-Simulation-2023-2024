using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectArrowsQuestStep : QuestStep
{
    private int arrowsCollected = 0;

    private int arrowsToComplete = 14;

    private void OnEnable()
    {
        Manager.Instance.miscEvents.onArrowCollected += ArrowCollected;
    }

    private void OnDisable()
    {
        Manager.Instance.miscEvents.onArrowCollected -= ArrowCollected;
    }

    void ArrowCollected()
    {
        if(arrowsCollected < arrowsToComplete)
        {
            arrowsCollected++;
            UpdateState();
        }

        if(arrowsCollected >= arrowsToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = arrowsCollected.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.arrowsCollected = System.Int32.Parse(state);
        UpdateState();
    }
}

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
        }

        if(arrowsCollected >= arrowsToComplete)
        {
            FinishQuestStep();
        }
    }
}

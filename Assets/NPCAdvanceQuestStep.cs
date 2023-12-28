using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAdvanceQuestStep : NPCStep
{
    bool stopMovement;

    public override void Execute()
    {
        foreach (var step in QuestManager.instance.gameObject.GetComponentsInChildren<QuestStep>())
        {
            if(step.questId == GetComponentInParent<QuestPoint>().questInfoForPoint.id)
            {
                Destroy(step.gameObject);
                Journal.Instance.RemoveQuest(step.questId);
            }
        }
        QuestManager.instance.AdvanceQuest(GetComponentInParent<QuestPoint>().questInfoForPoint.id);
        if(stopMovement)
        {
            npc.StopMovement();
        }
        npc.EndStep();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAdvanceQuestStep : NPCStep
{
    bool stopMovement;

    public override void Execute()
    {
        print("running");
        QuestManager.instance.AdvanceQuest(GetComponentInParent<QuestPoint>().questInfoForPoint.id);
        if(stopMovement)
        {
            npc.StopMovement();
        }
        npc.EndStep();
    }
}

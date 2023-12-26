using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndQuestNPCStep : NPCStep
{
    public bool waitingToFinish;

    public override void Execute()
    {
        QuestManager.instance.FinishQuest(npc.quest.questInfoForPoint.id);
        Finish();
    }

    public override void Finish()
    {
        npc.EndStep();
    }
}

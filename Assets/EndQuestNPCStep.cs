using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndQuestNPCStep : NPCStep
{
    public bool waitingToFinish;

    public override void Execute()
    {
        waitingToFinish = true;
    }

    public void Update()
    {
        if (npc.quest && waitingToFinish)
        {
            npc.quest.StartQuest();
            if(npc.quest.currentQuestState == QuestState.FINISHED)
            {
                Finish();
            }
        }
    }

    public override void Finish()
    {
        npc.EndStep();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStartNPCStep : NPCStep
{
    public override void Execute()
    {
        if (npc.quest)
        {
            npc.quest.StartQuest();
        }
        npc.EndStep();
    }
}

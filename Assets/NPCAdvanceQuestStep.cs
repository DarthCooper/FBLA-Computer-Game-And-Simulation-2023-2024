using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAdvanceQuestStep : NPCStep
{
    bool stopMovement;

    public override void Execute()
    {
        Manager.Instance.miscEvents.NpcPositionReached();
        if(stopMovement)
        {
            npc.StopMovement();
        }
        npc.EndStep();
    }
}

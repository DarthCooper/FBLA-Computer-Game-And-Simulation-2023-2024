using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStartTutorial : NPCStep
{
    public string tutorialId;

    public override void Execute()
    {
        if(npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        TutorialManager.instance.RunTutorial(tutorialId);
        npc.EndStep();
    }
}

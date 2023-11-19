using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextNPCStep : NPCStep
{
    public string text;

    public float timeToDisplay;

    public bool stopMovement;

    public override void Execute()
    {
        if(npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        npc.DisplayText(text);
        if(stopMovement)
        {
            npc.StopMovement();
        }
    }

    private void Update()
    {
        if(timeToDisplay > 0)
        {
            timeToDisplay -= Time.deltaTime;
        }else
        {
            Finish();
        }
    }

    public override void Finish()
    {
        if (npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        npc.HideText();
        npc.EndStep();
    }
}

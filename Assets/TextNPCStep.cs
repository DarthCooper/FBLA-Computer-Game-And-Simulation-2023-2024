using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextNPCStep : NPCStep
{
    public string speaker = "Carrot";

    [TextAreaAttribute]public string text;

    public float timeToDisplay;

    public bool stopMovement;

    public override void Execute()
    {
        if(npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        npc.DisplayText(text, speaker);
        if(stopMovement)
        {
            npc.StopMovement();
        }
    }

    private void Update()
    {

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

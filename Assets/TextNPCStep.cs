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

    public bool changeCamera;

    public bool resetCamera;

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

        if(changeCamera)
        {
            GameObject switcher = GameObject.Find("CameraSwitcher");
            if(switcher != null)
            {
                CameraSwitcher camSwitch = switcher.GetComponent<CameraSwitcher>();
                if(camSwitch != null)
                {
                    camSwitch.SwitchCamera(speaker);
                }
            }
        }
    }

    private void Update()
    {

    }

    public override void Finish()
    {
        if (resetCamera)
        {
            GameObject switcher = GameObject.Find("CameraSwitcher");
            if (switcher != null)
            {
                CameraSwitcher camSwitch = switcher.GetComponent<CameraSwitcher>();
                if (camSwitch != null)
                {
                    camSwitch.SwitchCamera("You");
                }
            }
        }
        if (npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        npc.HideText();
        npc.EndStep();
    }
}

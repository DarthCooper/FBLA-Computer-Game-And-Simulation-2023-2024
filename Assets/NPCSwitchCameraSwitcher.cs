using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSwitchCameraSwitcher : NPCStep
{
    public string name;

    public override void Execute()
    {
        if (npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        GameObject switcher = GameObject.Find("CameraSwitcher");
        if (switcher != null)
        {
            CameraSwitcher camSwitch = switcher.GetComponent<CameraSwitcher>();
            if (camSwitch != null)
            {
                camSwitch.setValue(name, npc.transform);
            }
        }
        npc.EndStep();
    }
}

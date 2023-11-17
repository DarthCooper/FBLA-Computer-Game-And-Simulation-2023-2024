using Mirror.Examples.BenchmarkIdle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNPCStep : NPCStep
{
    public string TargetName;

    public override void Execute()
    {
        if (npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        npc.StartMovement(TargetName);
    }
}

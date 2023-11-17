using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStep : MonoBehaviour
{
    public NPC npc = null;

    public void SetNpc(NPC npc)
    {
        this.npc = npc;
    }

    public virtual void Execute()
    {

    }

    public virtual void Finish()
    {

    }
}

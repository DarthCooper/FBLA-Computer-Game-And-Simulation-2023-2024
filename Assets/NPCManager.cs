using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

public class NPCManager : NetworkBehaviour
{
    public static Dictionary<string, bool> NPCsCompleted = new Dictionary<string, bool>();

    private void Update()
    {
        if(!isServer) { return; }
        var NPCs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
        foreach (var npc in NPCs)
        {
            if(!NPCsCompleted.ContainsKey(npc.NPCName))
            {
                if(npc.currentStepIndex < npc.steps.Length)
                {
                    NPCsCompleted.Add(npc.NPCName, false);
                    SyncNPCs(npc.NPCName, false);
                }else
                {
                    NPCsCompleted.Add(npc.NPCName, true);
                    SyncNPCs(npc.NPCName, true);
                }
            }
        }
    }

    [ClientRpc(includeOwner = false)]
    public void SyncNPCs(string name, bool completed)
    {
        if(NPCsCompleted.ContainsKey(name)) { NPCsCompleted[name] = completed; return; }
        NPCsCompleted.Add(name, completed);
    }

    public static void CompleteNPC(string name)
    {
        NPCsCompleted[name] = true;
    }
}

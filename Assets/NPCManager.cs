using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static Dictionary<string, bool> NPCsCompleted = new Dictionary<string, bool>();

    private void Update()
    {
        var NPCs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
        foreach (var npc in NPCs)
        {
            if(!NPCsCompleted.ContainsKey(npc.NPCName))
            {
                if(npc.currentStepIndex < npc.steps.Length)
                {
                    NPCsCompleted.Add(npc.NPCName, false);
                }else
                {
                    NPCsCompleted.Add(npc.NPCName, true);
                }
            }
        }
    }

    public static void CompleteNPC(string name)
    {
        NPCsCompleted[name] = true;
    }
}

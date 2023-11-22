using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistantNPC : MonoBehaviour, IDataPersistence
{

    public SerializableDictionary<string, Vector3> NPCPos;
    public SerializableDictionary<string, int> NPCStep;

    public List<NPC> NPCs = new List<NPC>();

    public void LoadData(GameData data)
    {
        LoadNPCPos(data);
        LoadNPCStep(data);
    }

    public void LoadNPCPos(GameData data)
    {
        this.NPCPos = data.NPCPos;
        foreach (var id in NPCPos.Keys)
        {
            foreach (var NPC in NPCs)
            {
                if (id == NPC.NPCName)
                {
                    NPC.transform.position = NPCPos[id];
                }
            }
        }
    }

    public void LoadNPCStep(GameData data)
    {
        this.NPCStep = data.NPCStep;
        foreach (var id in NPCStep.Keys)
        {
            foreach(var NPC in NPCs)
            {
                if(id == NPC.NPCName)
                {
                    NPC.currentStepIndex = NPCStep[id];
                    if (NPCStep[id] >= NPC.steps.Length)
                    {
                        NPC.OnFinishSteps.Invoke();
                    }
                }
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        SaveNPCPos(ref data);
        SaveNPCStep(ref data);
    }

    public void SaveNPCPos(ref GameData data)
    {
        foreach (var NPC in NPCs)
        {
            if (data.NPCPos.ContainsKey(NPC.NPCName))
            {
                data.NPCPos.Remove(NPC.NPCName);
            }
            data.NPCPos.Add(NPC.NPCName, NPC.transform.position);
        }
    }

    public void SaveNPCStep(ref GameData data)
    {
        foreach(var NPC in NPCs)
        {
            if(data.NPCStep.ContainsKey(NPC.NPCName))
            {
                data.NPCStep.Remove(NPC.NPCName);
            }
            data.NPCStep.Add(NPC.NPCName, NPC.currentStepIndex);
        }
    }
}

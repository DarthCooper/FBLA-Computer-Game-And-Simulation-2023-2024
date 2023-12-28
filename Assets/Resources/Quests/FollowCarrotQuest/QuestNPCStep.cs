using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPCStep : QuestStep
{
    public float distance;

    public string[] NPCnames;

    public NPC npc;
    public GameObject player;

    public float tolerance = 5f;

    public bool waiting;

    private void OnEnable()
    {
        Manager.Instance.miscEvents.onPositionReached += ReachedPosition;
    }

    private void OnDisable()
    {
        Manager.Instance.miscEvents.onPositionReached -= ReachedPosition;
    }

    void ReachedPosition()
    {
    }

    private void Update()
    {
        if(!npc)
        {
            GetNPC();
        }
        if(!player)
        {
            GetPlayer();
        }
        if(npc && player)
        {
            distance = Vector3.Distance(npc.transform.position, player.transform.position);
            difference = npc.transform.position - player.transform.position;
            SetProgress();
        }
        else
        {
            progress = "In another area";
        }
        if(distance < tolerance && waiting)
        {
            FinishQuestStep();
        }
    }

    public void SetProgress()
    {
        progress = "Distance from carrot: " + distance.ToString();
    }

    public void GetNPC()
    {
        NPC[] npcs = GameObject.FindObjectsByType<NPC>(FindObjectsSortMode.None);
        foreach (NPC npc in npcs)
        {
            if(NPCManager.NPCsCompleted.ContainsKey(npc.NPCName))
            {
                if (NPCManager.NPCsCompleted[npc.NPCName])
                {
                    continue;
                }
            }
            foreach (string name in NPCnames)
            {
                if(npc.NPCName == name && npc.canRun)
                {
                    this.npc = npc;
                }
            }
        }
    }

    public void GetPlayer()
    {
        player = GameObject.Find("LocalGamePlayer");
    }

    private void UpdateState()
    {
        string state = distance.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.distance = System.Int32.Parse(state);
        UpdateState();
    }
}

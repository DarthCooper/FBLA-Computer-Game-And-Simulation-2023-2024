using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMushroomsNPCQuestStep : QuestStep
{
    private int mushroomsCollected = 0;

    private int mushroomsTilComplete = 5;


    private void OnEnable()
    {
        Manager.Instance.miscEvents.onMushroomCollected += MushroomCollected;
    }

    private void OnDisable()
    {
        Manager.Instance.miscEvents.onMushroomCollected -= MushroomCollected;
    }

    void MushroomCollected()
    {
        if (mushroomsCollected < mushroomsTilComplete)
        {
            mushroomsCollected++;
            progress = ("Mushrooms: " + mushroomsCollected + "/" + mushroomsTilComplete);
            Journal.Instance.DisplayQuests();
            UpdateState();
        }
        if (mushroomsCollected >= mushroomsTilComplete)
        {
            FinishQuestStep();
        }
    }

    private void Update()
    {
        progress = ("Mushrooms: " + mushroomsCollected + "/" + mushroomsTilComplete);
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeMushroomsCollected()
    {
        ServerChangeMushrooms();
    }

    [Server]
    public void ServerChangeMushrooms()
    {
        RpcChangeMushrooms();
    }

    [ClientRpc]
    public void RpcChangeMushrooms()
    {
        mushroomsCollected++;
        progress = ("Mushrooms: " + mushroomsCollected + "/" + mushroomsTilComplete);
        Journal.Instance.DisplayQuests();
        if (isClient)
        {
            if (mushroomsCollected >= mushroomsTilComplete)
            {
                FinishQuestStep();
            }
        }
    }

    private void UpdateState()
    {
        string state = mushroomsCollected.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.mushroomsCollected = System.Int32.Parse(state);
        UpdateState();
    }
}

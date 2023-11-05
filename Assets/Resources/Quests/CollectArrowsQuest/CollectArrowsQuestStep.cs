using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CollectArrowsQuestStep : QuestStep
{
    private int arrowsCollected = 0;

    private int arrowsToComplete = 14;


    private void OnEnable()
    {
        Manager.Instance.miscEvents.onArrowCollected += ArrowCollected;
    }

    private void OnDisable()
    {
        Manager.Instance.miscEvents.onArrowCollected -= ArrowCollected;
    }

    void ArrowCollected()
    {
        if(arrowsCollected < arrowsToComplete)
        {
            arrowsCollected++;
            progress = ("" + arrowsCollected + "/" + arrowsToComplete);
            Journal.Instance.DisplayQuests();
            UpdateState();
        }
        if (arrowsCollected >= arrowsToComplete)
        {
            FinishQuestStep();
        }
    }

    private void Update()
    {
        progress = ("" + arrowsCollected + "/" + arrowsToComplete);
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeArrowsCollected()
    {
        ServerChangeArrows();
    }

    [Server]
    public void ServerChangeArrows()
    {
        RpcChangeArrows();
    }

    [ClientRpc]
    public void RpcChangeArrows()
    {
        arrowsCollected++;
        progress = ("" + arrowsCollected + "/" + arrowsToComplete);
        Journal.Instance.DisplayQuests();
        if(isClient)
        {
            if (arrowsCollected >= arrowsToComplete)
            {
                FinishQuestStep();
            }
        }
    }

    private void UpdateState()
    {
        string state = arrowsCollected.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.arrowsCollected = System.Int32.Parse(state);
        UpdateState();
    }
}

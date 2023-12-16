using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefearEnemiesNPCQuest : QuestStep
{
    public float enemiesDefeated;
    public float totalEnemies;

    private void OnEnable()
    {
        Manager.Instance.miscEvents.onEnemyKilled += EnemyKilled;
    }

    private void OnDisable()
    {
        Manager.Instance.miscEvents.onEnemyKilled -= EnemyKilled;
    }

    void EnemyKilled()
    {
        if (enemiesDefeated < totalEnemies)
        {
            enemiesDefeated++;
            SetProgress();
            Journal.Instance.DisplayQuests();
            UpdateState();
        }
        if (enemiesDefeated >= totalEnemies)
        {
            FinishQuestStep();
        }
    }

    public void SetProgress()
    {
        progress = ("Enemies: " + enemiesDefeated + "/" + totalEnemies);
    }

    private void Update()
    {
        SetProgress();
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
        enemiesDefeated--;
        SetProgress();
        Journal.Instance.DisplayQuests();
        if (isClient)
        {
            if (enemiesDefeated <= 0)
            {
                FinishQuestStep();
            }
        }
    }

    private void UpdateState()
    {
        string state = enemiesDefeated.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
    }
}

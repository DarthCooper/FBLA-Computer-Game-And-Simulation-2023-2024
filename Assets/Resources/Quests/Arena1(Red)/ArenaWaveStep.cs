using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaWaveStep : QuestStep
{
    public float time;
    public float enemiesLeft;
    public float totalEnemies;

    private void OnEnable()
    {
        Manager.Instance.miscEvents.onWaveEnemyKilled += EnemyKilled;
    }

    private void OnDisable()
    {
        Manager.Instance.miscEvents.onWaveEnemyKilled -= EnemyKilled;
    }

    void EnemyKilled()
    {
        if (enemiesLeft > 0)
        {
            enemiesLeft--;
            SetProgress();
            Journal.Instance.DisplayQuests();
            UpdateState();
        }
        if (enemiesLeft <= 0)
        {
            FinishQuestStep();
        }
    }

    public void SetProgress()
    {
        progress = ("Enemies: " + enemiesLeft + "/" + totalEnemies + "\n time: " + (int)time);
    }

    private void Update()
    {
        SetProgress();
        time -= Time.deltaTime;
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
        enemiesLeft--;
        SetProgress();
        Journal.Instance.DisplayQuests();
        if (isClient)
        {
            if (enemiesLeft <= 0)
            {
                FinishQuestStep();
            }
        }
    }

    private void UpdateState()
    {
        string state = enemiesLeft.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
    }
}

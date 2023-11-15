using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WaveSpawner : NetworkBehaviour
{
    public Transform[] spawnPoints;

    public float maxTimePerWave;
    public float curTimeInWave;

    public List<GameObject> Enemies = new List<GameObject>();

    [SerializeField] public Wave[] waves;

    public int currentWave;

    bool currentWavedSpawned;

    public QuestPoint quest;

    public string questID;

    private void Update()
    {
        CheckEnemiesLength();
        if(!currentWavedSpawned && (curTimeInWave <= 0 || Enemies.Count <= 0 )&& currentWave < waves.Length)
        {
            currentWavedSpawned = true;
            SetWaveStats();
            Invoke(nameof(spawnWave), 10f);
        }else if(curTimeInWave > 0 && currentWave < waves.Length)
        {
            curTimeInWave -= Time.deltaTime;
        }else if(currentWave >= waves.Length && Enemies.Count <= 0)
        {
            Debug.Log("You have won!");
        }
        QuestStep[] steps= GameObject.FindObjectsOfType<QuestStep>();
        for(int i = 0; i < steps.Length; i++)
        {
            if (steps[i].GetType().Equals(typeof(ArenaWaveStep)))
            {
                steps[i].GetComponent<ArenaWaveStep>().time = curTimeInWave;
            }
        }
    }

    public void SetWaveStats()
    {
        maxTimePerWave = waves[currentWave].TimeToComplete;
        curTimeInWave = maxTimePerWave;
    }

    public void CheckEnemiesLength()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i] != null)
            {
                if (Enemies[i].GetComponent<Health>())
                {
                    if (Enemies[i].GetComponent<Health>().health <= 0)
                    {
                        Enemies.RemoveAt(i);
                        Manager.Instance.miscEvents.WaveEnemyKilled();
                    }
                }
            }
        }
    }

    public void spawnWave()
    {
        if(quest)
        {
            quest.StartQuest();
        }
        if(!isServer) { return; }
        List<Transform> NewSpawnPoints = new List<Transform>(spawnPoints);
        for(int i = 0; i < waves[currentWave].enemies.Length; i++)
        {
            if(NewSpawnPoints.Count <= 0)
            {
                NewSpawnPoints = new List<Transform>(spawnPoints);
            }
            int spawnpointIndex = Random.Range(0, NewSpawnPoints.Count - 1);
            GameObject enemy = Instantiate(waves[currentWave].enemies[i], NewSpawnPoints[spawnpointIndex].position, Quaternion.identity);
            NewSpawnPoints.RemoveAt(spawnpointIndex);
            NetworkServer.Spawn(enemy);
            Enemies.Add(enemy);
        }
        currentWave++;
        currentWavedSpawned = false;
    }
}

[System.Serializable]
public class Wave
{
    public float TimeToComplete;
    public GameObject[] enemies;
}

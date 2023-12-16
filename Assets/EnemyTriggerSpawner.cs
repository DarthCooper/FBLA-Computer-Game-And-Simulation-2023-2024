using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerSpawner : NetworkBehaviour
{
    public EnemyAI[] enemies;

    public Transform[] spawnpoints;

    bool spawned;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && !spawned)
        {
            SpawnEnemies();
        }
    }

    public void SpawnEnemies()
    {
        CmdSpawnEnemies();
    }

    [Command(requiresAuthority = false)]
    public void CmdSpawnEnemies()
    {
        List<Transform> NewSpawnPoints = new List<Transform>(spawnpoints);
        foreach (var enemy in enemies)
        {
            if (NewSpawnPoints.Count <= 0)
            {
                NewSpawnPoints = new List<Transform>(spawnpoints);
            }
            int spawnpointIndex = Random.Range(0, NewSpawnPoints.Count - 1);
            GameObject spawnedEnemy = Instantiate(enemy.gameObject, NewSpawnPoints[spawnpointIndex].position, Quaternion.identity);
            spawnedEnemy.GetComponent<EnemyAI>().currentState = EnemyStates.Hunting;
            NewSpawnPoints.RemoveAt(spawnpointIndex);
            NetworkServer.Spawn(spawnedEnemy);
        }
        spawned = true;
    }
}

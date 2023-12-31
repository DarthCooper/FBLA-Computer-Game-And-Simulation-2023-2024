using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnEnemyStep : NPCStep
{
    public string enemySpawnerName;

    public override void Execute()
    {
        if(npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        foreach(EnemyTriggerSpawner spawner in FindObjectsByType<EnemyTriggerSpawner>(FindObjectsSortMode.None))
        {
            if(spawner.name == enemySpawnerName)
            {
                spawner.SpawnEnemies();
            }
        }
        npc.EndStep();
    }
}

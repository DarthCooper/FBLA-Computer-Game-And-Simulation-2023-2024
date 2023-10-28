using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistantEnemy : MonoBehaviour, IDataPersistence
{
    public SerializableDictionary<string, float> EnemiesHealth;
    public SerializableDictionary<string, Vector3> EnemiesPos;

    public List<EnemyAI> enemies = new List<EnemyAI>();

    public void LoadData(GameData data)
    {
        LoadEnemiesHealth(data);
        LoadEnemiesPos(data);
    }

    public void LoadEnemiesHealth(GameData data)
    {
        this.EnemiesHealth = data.EnemiesHealth;
        foreach (var id in EnemiesHealth.Keys)
        {
            foreach (var enemy in enemies)
            {
                if (id == enemy.id)
                {
                    enemy.GetComponent<Health>().SetHealth(EnemiesHealth[id]);
                }
            }
        }
    }

    public void LoadEnemiesPos(GameData data)
    {
        this.EnemiesPos = data.EnemiesPos;
        foreach (var id in EnemiesPos.Keys)
        {
            foreach (var enemy in enemies)
            {
                if (id == enemy.id)
                {
                    enemy.transform.position = EnemiesPos[id];
                }
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        SaveEnemiesHealth(ref data);
        SaveEnemiesPos(ref data);
    }

    public void SaveEnemiesHealth(ref GameData data)
    {
        foreach (var enemy in enemies)
        {
            if (data.EnemiesHealth.ContainsKey(enemy.id))
            {
                data.EnemiesHealth.Remove(enemy.id);
            }
            data.EnemiesHealth.Add(enemy.id, enemy.GetComponent<Health>().health);
        }
    }

    public void SaveEnemiesPos(ref GameData data)
    {
        foreach (var enemy in enemies)
        {
            if (data.EnemiesPos.ContainsKey(enemy.id))
            {
                data.EnemiesPos.Remove(enemy.id);
            }
            data.EnemiesPos.Add(enemy.id, enemy.transform.position);
        }
    }
}

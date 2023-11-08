using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    public List<string> items;

    public string lastScene;

    public Vector3 playerPosition;

    public string selectedQuest;

    public SerializableDictionary<string, bool> itemsCollected;

    public SerializableDictionary<string, float> EnemiesHealth;
    public SerializableDictionary<string, Vector3> EnemiesPos;

    public string primaryWeapon;
    public string secondaryWeapon;
    public string ammo;
    public string consumable;

    public float health;
    public float maxHealth;

    public SerializableDictionary<string, string> questData;

    // the values definied in this constructor will be the default values the game starts with when there's no data to load
    public GameData()
    {
        items = new List<string>();
        lastScene = "Opening";
        playerPosition = Vector3.zero;
        itemsCollected = new SerializableDictionary<string, bool>();
        EnemiesHealth = new SerializableDictionary<string, float>();
        EnemiesPos = new SerializableDictionary<string, Vector3>();
        primaryWeapon = string.Empty;
        secondaryWeapon = string.Empty;
        ammo = string.Empty;
        health = 100;
        maxHealth = 100;
    }
}

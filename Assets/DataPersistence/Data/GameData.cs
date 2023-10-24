using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    public List<string> items;

    public Vector3 playerPosition;

    public SerializableDictionary<string, bool> itemsCollected;

    public string primaryWeapon;
    public string secondaryWeapon;
    public string ammo;

    // the values definied in this constructor will be the default values the game starts with when there's no data to load
    public GameData()
    {
        items = new List<string>();
        playerPosition = Vector3.zero;
        itemsCollected = new SerializableDictionary<string, bool>();
        primaryWeapon = string.Empty;
        secondaryWeapon = string.Empty;
        ammo = string.Empty;
    }
}

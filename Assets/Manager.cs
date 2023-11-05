using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using Mirror;

public class Manager : NetworkBehaviour
{
    public static Manager Instance;

    public PlayerObjectController[] players;

    public MiscEvents miscEvents;
    public QuestEvents questEvents;

    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }
        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        InvokeRepeating(nameof(CheckPlayersHealth), 0, 2f);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayers();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        DontDestroyOnLoad(gameObject);
    }

    public void FindPlayers()
    {
        players = FindObjectsOfType(typeof(PlayerObjectController)) as PlayerObjectController[];
    }

    public void ReadItem(Item item)
    {
        CmdReadItem(item.itemName);
        if(!isServer)
        {
            if (item.itemType == ItemType.Ammo)
            {
                miscEvents.ArrowCollected();
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdReadItem(string itemName)
    {
        ServerReadItem(itemName);
    }

    [Server]
    public void ServerReadItem(string itemName)
    {
        RpcReadItem(itemName);
    }

    [ClientRpc]
    public void RpcReadItem(string itemName)
    {
        Item item = Inventory.Instance.GetItem(itemName);
        if (item.itemType == ItemType.Ammo)
        {
            miscEvents.ArrowCollected();
        }
    }

    public void CheckPlayersHealth()
    {
        if(players.Length <= 0) { return; }
        bool allDead = true;
        foreach (PlayerObjectController player in players)
        {
            if(player.GetComponent<Health>() != null)
            {
                if(player.GetComponent<Health>().health > 0)
                {
                    allDead = false;
                }
            }
        }
        if(allDead)
        {
            DataPersistenceManager.instance.LoadGame();
            foreach (PlayerObjectController player in players)
            {
                player.GetComponent<Health>().SetHealth(player.GetComponent<Health>().maxHealth);
            }
            GameObject.FindObjectOfType<CustomNetworkManager>().StartGame("Game");
        }
    }
}

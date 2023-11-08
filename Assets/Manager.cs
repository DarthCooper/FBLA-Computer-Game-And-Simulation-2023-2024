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

    private CustomNetworkManager networkManager;

    private SceneSaver sceneSaver;

    public SceneSettings settings;

    public PlayerObjectController[] players;

    public MiscEvents miscEvents;
    public QuestEvents questEvents;

    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }
        FindSettings();
        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        InvokeRepeating(nameof(CheckPlayersHealth), 0, 2f);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        sceneSaver = GetComponent<SceneSaver>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayers();
        FindSettings();
    }

    private void Update()
    {
        if(settings == null)
        {
            FindSettings();
        }
    }

    public void OnSceneUnloaded(Scene scene)
    {
        DontDestroyOnLoad(gameObject);
    }

    public void FindPlayers()
    {
        players = FindObjectsOfType(typeof(PlayerObjectController)) as PlayerObjectController[];
    }

    public void FindSettings()
    {
        settings = FindObjectOfType(typeof(SceneSettings)) as SceneSettings;
    }

    public void ReadItem(Item item)
    {
        for(int i = 0; i < item.currentStack; i++)
        {
            if (item.itemType == ItemType.Ammo)
            {
                miscEvents.ArrowCollected();
            }
        }
    }

    public void CheckPlayersHealth()
    {
        FindPlayers();
        if(SceneManager.GetActiveScene().name != "Game") { return; }
        if(players.Length <= 0) { ReloadLevel(); return; }
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
            ReloadLevel();
        }
    }

    public void ReloadLevel()
    {
        DataPersistenceManager.instance.LoadGame();
        foreach (PlayerObjectController player in players)
        {
            player.GetComponent<Health>().SetHealth(player.GetComponent<Health>().maxHealth);
        }
        GameObject.FindObjectOfType<CustomNetworkManager>().StartGame("Game");
    }

    public void LoadNewLevel(string name)
    {
        networkManager.StartGame(name);
    }
}

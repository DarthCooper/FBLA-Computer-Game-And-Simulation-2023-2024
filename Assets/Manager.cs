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

    public Animator animator;

    [SyncVar(hook = nameof(SetDontDestroyOnLoad))]public bool destroyOnLoad;

    public bool AllowMovement = true;
    public bool AllowInteract = true;
    public bool AllowOtherInput = true;

    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this;
        }
        FindSettings();
        animator = GetComponentInChildren<Animator>();
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

    public void CurrentLoadedCanvasSheet(bool[] canvases, bool keepMovement, bool keepInteract, bool keepOtherInput)
    {
        bool AnyActive = false;
        foreach (var canvas in canvases)
        {
            if(canvas)
            {
                AnyActive = true;
            }
        }
        if(AnyActive)
        {
            AllowMovement = keepMovement;
            AllowInteract = keepInteract;
            AllowOtherInput = keepOtherInput;
        }else
        {
            AllowMovement = true;
            AllowInteract = true;
            AllowOtherInput = true;
        }
    }

    private void Start()
    {
        CmdSetDontDestroyOnLoad();
    }

    public GameObject GetLocalPlayer()
    {
        return GameObject.Find("LocalGamePlayer");
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
        if(Instance != this)
        {
            Instance = this;
        }
        if(settings == null)
        {
            FindSettings();
        }
        if(destroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
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
            if(item.itemName == "Mushroom")
            {
                miscEvents.MushroomCollected();
            }
        }
    }

    public void CheckPlayersHealth()
    {
        FindPlayers();
        if(!settings)
        if(!settings.isPlayable) { return; }
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
        foreach (PlayerObjectController player in players)
        {
            player.GetComponent<PlayerInventory>().setData = false;
        }
        DataPersistenceManager.instance.LoadGame();
        foreach (PlayerObjectController player in players)
        {
            player.GetComponent<Health>().SetHealth(player.GetComponent<Health>().maxHealth);
        }
        GameObject.FindObjectOfType<CustomNetworkManager>().StartGame(SceneManager.GetActiveScene().name);
    }

    public void LoadNewLevel(string name)
    {
        networkManager.StartGame(name);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetDontDestroyOnLoad()
    {
        SetDontDestroyOnLoad(destroyOnLoad, true);
    }

    public void SetDontDestroyOnLoad(bool oldValue, bool newValue)
    {
        destroyOnLoad = newValue;
    }

    public void SetPlayerPos(float x, float y)
    {
        CmdSetPlayerPos(x, y);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPlayerPos(float x, float y)
    {
        if(isServer)
        {
            ServerSetPlayerPos(x, y);
        }
    }

    [Server]
    public void ServerSetPlayerPos(float x, float y)
    {
        RpcSetPlayerPos(x, y);
    }

    [ClientRpc]
    public void RpcSetPlayerPos(float x, float y)
    {
        foreach(var player in players)
        {
            print("Setting");
            player.transform.position = new Vector3(x, y, 0);
        }
    }

    string sceneName;

    public void FadeToBlack(string sceneName)
    {
        this.sceneName = sceneName;
        animator.SetTrigger("ToBlack");
    }

    public void changeScene()
    {
        networkManager.ChangeScene(sceneName);
    }
}

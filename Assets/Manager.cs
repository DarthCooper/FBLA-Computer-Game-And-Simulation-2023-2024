using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public PlayerObjectController[] players;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        InvokeRepeating(nameof(CheckPlayersHealth), 0, 2f);
    }

    private void OnEnable()
    {
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
    }

    public void FindPlayers()
    {
        players = FindObjectsOfType(typeof(PlayerObjectController)) as PlayerObjectController[];
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

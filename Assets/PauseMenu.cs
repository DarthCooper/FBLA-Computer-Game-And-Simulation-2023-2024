using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void OnQuit()
    {
        Application.Quit();
        OnSaveGame();
    }

    public void OnLoadGame()
    {
        Manager.Instance.ReloadLevel();
    }

    public void OnSaveGame()
    {
        DataPersistenceManager.instance.SaveGame();
    }

    public void OnReturnToMenu()
    {
        OnSaveGame();
        List<GameObject> toDestroy = new List<GameObject>();
        toDestroy.Add(GameObject.Find("NetworkManager"));
        toDestroy.Add(GameObject.Find("GameManager"));
        toDestroy.Add(GameObject.Find("DataPersistenceManager"));
        foreach(GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            toDestroy.Add(playerObj);
        }
        foreach(var obj in toDestroy)
        {
            print(obj);
            if(obj != null)
            {
                Destroy(obj);
            }
        }
        GameObject player = GameObject.Find("LocalGamePlayer");
        if(player.GetComponent<NetworkIdentity>())
        {
            if(player.GetComponent<NetworkIdentity>().isServer)
            {
                CustomNetworkManager.singleton.StopHost();
            }else
            {
                CustomNetworkManager.singleton.StopClient();
            }
        }
    }
}

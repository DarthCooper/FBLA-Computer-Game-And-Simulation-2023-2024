using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if(manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if(isServer)
        {
            this.Ready = newValue;
        }
        if(isClient) 
        { 
            LobbyController.instance.UpdatePlayerList();
        }
    }

    [Command]
    void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    public void ChangeReady()
    {
        if(isOwned)
        {
            CmdSetPlayerReady();
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.instance.FindLocalPlayer();
        LobbyController.instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.instance.UpdateLobbyName();
        LobbyController.instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string name)
    {
        this.PlayerNameUpdate(this.PlayerName, name);
    }

    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if(isServer)
        {
            this.PlayerName = NewValue;
        }
        if(isClient)
        {
            LobbyController.instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string SceneName)
    {
        if(isOwned)
        {
            CmdCanStartGame(SceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Interactable : NetworkBehaviour
{
    public UnityEvent OnInteract;
    public UnityEvent OnEndInteract;

    public bool beenInteractedWith;

    public GameObject Player;

    public void Interact(GameObject Player)
    {
        CmdInteract();
        this.Player = Player;
    }

    [Command(requiresAuthority = false)]
    void CmdInteract()
    {
        ServerInteract();
    }

    [Server]
    void ServerInteract()
    {
        RpcInteract();
    }

    [ClientRpc]
    void RpcInteract()
    {
        OnInteract.Invoke();
    }

    public void EndInteract()
    {
        CmdEndInteract();
    }

    [Command(requiresAuthority = false)]
    void CmdEndInteract()
    {
        ServerEndInteract();
    }

    [Server]
    void ServerEndInteract()
    {
        if (isServer)
        {
            RpcEndInteract();
        }
    }

    [ClientRpc]
    void RpcEndInteract()
    {
        OnEndInteract.Invoke();
    }
}

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

    public void Interact()
    {
        CmdInteract();
    }

    [Command(requiresAuthority = false)]
    void CmdInteract()
    {
        ServerInteract();
    }

    [Server]
    void ServerInteract()
    {
        if(isServer)
        {
            RpcInteract();
        }
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
        OnInteract.Invoke();
    }
}

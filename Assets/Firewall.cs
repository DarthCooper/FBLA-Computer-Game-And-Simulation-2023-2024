using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Firewall : NetworkBehaviour
{
    bool canInteract;
    public void OnInteract()
    {
        if(canInteract)
        {
            ComputerPuzzle.Instance.EnableComputer(this);
            canInteract = false;
        }
    }

    public void OnEndInteract()
    {
        canInteract = true;
    }

    public void DisableFirewall()
    {
        CmdDisableFirewall();
    }

    [Command(requiresAuthority = false)]
    public void CmdDisableFirewall()
    {
        if(isServer)
        {
            ServerDisableFirewall();
        }
    }

    [Server]
    public void ServerDisableFirewall()
    {
        RpcDisableFirewall();
    }

    [ClientRpc]
    public void RpcDisableFirewall()
    {
        foreach (var item in GetComponents<Collider2D>())
        {
            item.enabled = false;
        }
        foreach (var item in GetComponents<SpriteRenderer>())
        {
            item.enabled = false;
        }
    }
}

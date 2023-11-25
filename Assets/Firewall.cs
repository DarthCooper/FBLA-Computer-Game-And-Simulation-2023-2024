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
        Transform closest = GetClosest();
        if(canInteract && closest.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            ComputerPuzzle.Instance.EnableComputer(this);
            canInteract = false;
        }
    }

    public Transform GetClosest()
    {
        float maxDistance = 10000;
        Transform Target = null;
        foreach (var target in GameObject.FindGameObjectsWithTag("Player"))
        {
            float Distance = Vector2.Distance(transform.position, target.transform.position);
            if (Distance < maxDistance)
            {
                maxDistance = Distance;
                Target = target.transform;
            }
        }
        return Target;
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

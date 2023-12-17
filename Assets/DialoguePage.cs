using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialoguePage : NetworkBehaviour
{
    public TMP_Text messageText;

    public TMP_Text speakerText;

    public NPC npc;

    public void SetMessage(string message)
    {
        CmdSetMessage(message);
    }


    [Command(requiresAuthority = false)]
    public void CmdSetMessage(string message)
    {
        if (isServer)
        {
            ServerSetMessage(message);
        }
    }

    [Server]
    public void ServerSetMessage(string message)
    {
        RpcSetMessage(message);
    }

    [ClientRpc]
    public void RpcSetMessage(string message)
    {
        messageText.text = message;
        GetClosest();
    }

    public void OnClick()
    {
        CmdOnClick();
    }

    [Command(requiresAuthority = false)]
    public void CmdOnClick()
    {
        if(isServer)
        {
            ServerOnClick();
        }
    }

    [Server]
    public void ServerOnClick()
    {
        RpcOnClick();
    }

    [ClientRpc]
    public void RpcOnClick()
    {
        if(npc == null)
        {
            GetClosest();
        }
        npc.currentStep.Finish();
    }

    public virtual void GetClosest()
    {
        GameObject player = GameObject.Find("LocalGamePlayer");
        float maxDistance = 10000;
        foreach (var target in GameObject.FindGameObjectsWithTag("NPC"))
        {
            float Distance = Vector2.Distance(player.transform.position, target.transform.position);
            if (Distance < maxDistance)
            {
                maxDistance = Distance;
                npc = target.GetComponent<NPC>();
            }
        }
    }
}

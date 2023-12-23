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

    string npcName;

    public void SetMessage(string message, string speaker, GameObject NPC)
    {
        CmdSetMessage(message, speaker, NPC.name);
    }


    [Command(requiresAuthority = false)]
    public void CmdSetMessage(string message, string speaker, string NPCname)
    {
        if (isServer)
        {
            ServerSetMessage(message, speaker, NPCname);
        }
    }

    [Server]
    public void ServerSetMessage(string message, string speaker, string NPCname)
    {
        RpcSetMessage(message, speaker, NPCname);
    }

    [ClientRpc]
    public void RpcSetMessage(string message, string speaker, string NPCname)
    {
        messageText.text = message;
        speakerText.text = speaker;
        var npcObject = GameObject.Find(NPCname);
        this.npcName = NPCname;
        if(npcObject != null)
        {
            npc = npcObject.GetComponent<NPC>();
        }
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
            var npcObject = GameObject.Find(npcName);
            if (npcObject != null)
            {
                npc = npcObject.GetComponent<NPC>();
            }
        }
        if(npc)
        {
            npc.currentStep.Finish();
        }
    }

    public virtual void GetClosest()
    {
        GameObject player = GameObject.Find("LocalGamePlayer");
        float maxDistance = 10000;
        foreach (var target in GameObject.FindGameObjectsWithTag("NPC"))
        {
            if(target.GetComponent<NPC>().disabled) { return; }
            float Distance = Vector2.Distance(player.transform.position, target.transform.position);
            if (Distance < maxDistance)
            {
                maxDistance = Distance;
                npc = target.GetComponent<NPC>();
            }
        }
    }
}

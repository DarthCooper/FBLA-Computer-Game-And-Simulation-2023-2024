using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Interactable : NetworkBehaviour
{
    public string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public UnityEvent OnInteract;
    public UnityEvent OnEndInteract;

    public GameObject SelectableGraphics;

    public bool beenInteractedWith;

    public GameObject Player;

    public bool collected;

    public void Interact(GameObject Player)
    {
        if(collected) { return; }
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

    public void collect()
    {
        collected = true;
    }

    public void LoadData(GameData data)
    {
        print("Load");
        data.itemsCollected.TryGetValue(id, out collected);
        if(collected)
        {
            GetComponent<Item>().DisableObject();
        }
    }

    public void SaveData(ref GameData data)
    {
        if(data.itemsCollected.ContainsKey(id))
        {
            data.itemsCollected.Remove(id);
        }
        data.itemsCollected.Add(id, collected);
        print("Saved");
    }

    public void ChangeSelectableView(bool inRange)
    {
        if (collected) { inRange = false; }
        SelectableGraphics.SetActive(inRange);
    }
}

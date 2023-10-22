using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Item : NetworkBehaviour
{
    public string itemName;
    public GameObject itemPrefab;
    public Texture2D itemTexture;

    Interactable interactable;

    public ItemType itemType;

    public int inventoryIndex;

    public float delayBetweenUses;

    public bool stackable;
    public int maxStack;

    public int currentStack = 1;

    public GameObject projectile;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    public void PickUp()
    {
        if(interactable.Player)
        {
            if(interactable.Player.GetComponent<NetworkIdentity>().isOwned)
            {
                Inventory.Instance.AddItem(this);
            }
        }
        Destroy(this.gameObject);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetStack(int stack)
    {
        ServerSetStack(stack);
    }

    [Server]
    public void ServerSetStack(int stack)
    {
        RpcSetStack(stack);
    }

    [ClientRpc]
    public void RpcSetStack(int stack)
    {
        currentStack = stack;
    }
}
public enum ItemType
{
    Weapon,
    Equipable,
    Useable,
    Ammo,
    Static
}

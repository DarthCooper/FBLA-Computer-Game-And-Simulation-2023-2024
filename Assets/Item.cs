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

    public Component[] componentsToDisable;

    bool beenPickedUp = false;

    public int price;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    public void PickUp()
    {
        if(interactable.Player && !beenPickedUp)
        {
            if(interactable.Player.GetComponent<NetworkIdentity>().isOwned)
            {
                Inventory.Instance.AddItem(this);
            }
            beenPickedUp = true;
            interactable.beenInteractedWith = true;
        }
        DisableObject();
    }

    public void ReadItem()
    {
        if(interactable.beenInteractedWith) { return; }
        Manager.Instance.ReadItem(this);
        interactable.beenInteractedWith = true;
    }

    public void DisableObject()
    {
        foreach(var component in componentsToDisable)
        {
            DisableComponent(component);
        }
    }

    void DisableComponent(Component component)
    {
        if (component is Renderer)
        {
            (component as Renderer).enabled = false;
        }
        else if (component is Collider2D)
        {
            (component as Collider2D).enabled = false;
        }else if(component is Transform)
        {
            component.GetComponent<SpriteRenderer>().enabled = false;
        }
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

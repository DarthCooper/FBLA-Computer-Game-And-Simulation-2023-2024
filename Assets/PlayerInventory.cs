using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class PlayerInventory : NetworkBehaviour
{
    PlayerAttack attack;

    public Item PrimaryWeapon;
    public Item SecondaryWeapon;

    public Item Ammo;

    private void Awake()
    {
        attack = GetComponent<PlayerAttack>();
    }

    #region Set Primary Weapon
    public void SetPrimaryItem(Item item)
    {
        CmdSetPrimary(item.itemName);
    }
    [Command(requiresAuthority = false)]
    public void CmdSetPrimary(string item)
    {
        if(isServer)
        {
            ServerSetPrimary(item);
        }
    }
    [Server]
    public void ServerSetPrimary(string item)
    {
        RpcSetPrimary(item);
    }
    [ClientRpc]
    public void RpcSetPrimary(string itemName)
    {
        PrimaryWeapon = Inventory.Instance.GetItem(itemName);
        attack.ChangePrimaryAttack(itemName);
    }
    #endregion

    #region Set Secondary Weapon
    public void SetSecondaryItem(Item item)
    {
        CmdSetSecondary(item.itemName);
    }
    [Command(requiresAuthority = false)]
    public void CmdSetSecondary(string item)
    {
        if(isServer)
        {
            ServerSetSecondary(item);
        }
    }
    [Server]
    public void ServerSetSecondary(string item)
    {
        RpcSetSecondary(item);
    }
    [ClientRpc]
    public void RpcSetSecondary(string itemName)
    {
        SecondaryWeapon = Inventory.Instance.GetItem(itemName);
        attack.ChangeSecondaryAttack(itemName);
    }
    #endregion

    #region Set Ammo
    public void SetAmmo(Item item)
    {
        CmdSetAmmo(item.itemName, item.currentStack);
    }
    [Command(requiresAuthority = false)]
    public void CmdSetAmmo(string item, int stack)
    {
        if (isServer)
        {
            ServerSetAmmo(item, stack);
        }
    }
    [Server]
    public void ServerSetAmmo(string item, int stack)
    {
        RpcSetAmmo(item, stack);
    }
    [ClientRpc]
    public void RpcSetAmmo(string itemName, int stack)
    {
        Ammo = Inventory.Instance.GetItem(itemName);
        if(itemName != "")
        {
            Ammo.currentStack = stack;
        }
    }

    #endregion 

    public void spawnDroppedItem(string ItemName, int AmountDropped)
    {
        CmdSpawnDroppedItem(ItemName, AmountDropped);
    }

    [Command(requiresAuthority = false)]
    public void CmdSpawnDroppedItem(string ItemName, int AmountDropped)
    {
        var droppedObject = Instantiate(Inventory.Instance.GetItem(ItemName).gameObject, transform.position, Quaternion.identity);
        NetworkServer.Spawn(droppedObject);
        droppedObject.GetComponent<Item>().CmdSetStack(AmountDropped);
    }
}

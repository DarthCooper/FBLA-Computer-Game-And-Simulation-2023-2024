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
}

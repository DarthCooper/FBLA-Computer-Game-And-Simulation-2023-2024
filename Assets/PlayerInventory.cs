using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerInventory : NetworkBehaviour, IDataPersistence
{
    PlayerAttack attack;

    public Item PrimaryWeapon;
    public Item SecondaryWeapon;

    public Item Ammo;

    public Item Consumable;

    GameData gameData;

    public string primaryTest;

    private void Awake()
    {
        attack = GetComponent<PlayerAttack>();
    }

    #region Set Primary Weapon
    public void SetPrimaryItem(Item item)
    {
        if(!item || item.itemName == string.Empty) { CmdSetPrimary(""); return; }
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
        if (!item || item.itemName == string.Empty) { CmdSetSecondary(""); return; }
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
        if (!item || item.itemName == string.Empty) { CmdSetAmmo("", 0); return; }
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

    #region Set Consumable
    public void SetConsumable(Item item)
    {
        if (!item || item.itemName == string.Empty) { CmdSetConsumable("", 0); return; }
        CmdSetConsumable(item.itemName, item.currentStack);
    }
    [Command(requiresAuthority = false)]
    public void CmdSetConsumable(string item, int stack)
    {
        if (isServer)
        {
            ServerSetConsumable(item, stack);
        }
    }
    [Server]
    public void ServerSetConsumable(string item, int stack)
    {
        RpcSetConsumable(item, stack);
    }
    [ClientRpc]
    public void RpcSetConsumable(string itemName, int stack)
    {
        Consumable = Inventory.Instance.GetItem(itemName);
        if (itemName != "")
        {
            Consumable.currentStack = stack;
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

    public void LoadData(GameData data)
    {
        gameData = data;
        Invoke("SetData", 1f);
    }

    public void SetData()
    {
        if (gameData.primaryWeapon != string.Empty)
        {
            Inventory.Instance.PrimarySlot.EquipItem(Inventory.Instance.GetClosestSlot(Inventory.Instance.GetItem(gameData.primaryWeapon)));
        }
        if (gameData.secondaryWeapon != string.Empty)
        {
            Inventory.Instance.SecondarySlot.EquipItem(Inventory.Instance.GetClosestSlot(Inventory.Instance.GetItem(gameData.secondaryWeapon)));
        }
        if (gameData.ammo != string.Empty)
        {
            Inventory.Instance.AmmoSlot.EquipItem(Inventory.Instance.GetClosestSlot(Inventory.Instance.GetItem(gameData.ammo)));
        }
        if(gameData.consumable != string.Empty)
        {
            Inventory.Instance.ConsumableSlot.EquipItem(Inventory.Instance.GetClosestSlot(Inventory.Instance.GetItem(gameData.consumable)));
        }
    }

    public void SaveData(ref GameData data)
    {
        if(!isLocalPlayer) return;
        if(!PrimaryWeapon) { data.primaryWeapon = string.Empty; } else { data.primaryWeapon = PrimaryWeapon.itemName; }
        if(!SecondaryWeapon) { data.secondaryWeapon = string.Empty; } else { data.secondaryWeapon = SecondaryWeapon.itemName; }
        if(!Ammo) { data.ammo = string.Empty; } else { data.ammo = Ammo.itemName; }
        if(!Consumable) { data.consumable = string.Empty; } else { data.consumable = Consumable.itemName; }
    }
}

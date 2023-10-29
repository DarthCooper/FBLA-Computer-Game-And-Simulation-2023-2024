using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class PlayerAttack : NetworkBehaviour
{
    public bool Attacking;
    public bool secondaryAttacking;

    public bool usingConsumable;

    public UnityEvent OnPrimaryPressed;
    public UnityEvent OnSecondaryPressed;

    public UnityEvent OnConsumableUsed;

    [SyncVar(hook = "SetAmmo")] public int Ammo;
    [SyncVar(hook = "SetConsumables")] public int ConsumableAmount;

    Animator anim;

    bool canAttack = true;
    bool canSecondaryAttack = true;

    public bool canUseConsumable = true;

    public float timeBetweenAttack = 1f;
    public float timeBetweenSecondaryAttack = 1f;

    public float timeBetweenConsumableUse = 1f;

    public GameObject[] projectiles;
    public Transform firepoint;

    bool primary;

    public Dictionary<string, UnityAction> Attacks = new Dictionary<string, UnityAction>();
    public Dictionary<string, UnityAction> Consumables = new Dictionary<string, UnityAction>();

    private void Awake()
    {
        anim = GetComponent<Animator>();
        Attacks.Add("", nullAttack);
        Attacks.Add("Sword", meleeAttack);
        Attacks.Add("Bow", rangedAttack);
        Attacks.Add("Shotgun", shotgunAttack);

        Consumables.Add("", nullAttack);
        Consumables.Add("SmallHealth", SmallHealth);
    }

    public void ChangePrimaryAttack(string weapon)
    {
        OnPrimaryPressed.RemoveAllListeners();
        OnPrimaryPressed.AddListener(Attacks[weapon]);
        if(GetComponent<PlayerInventory>().PrimaryWeapon)
        {
            timeBetweenAttack = GetComponent<PlayerInventory>().PrimaryWeapon.delayBetweenUses;
        }
    }

    public void ChangeSecondaryAttack(string weapon)
    {
        OnSecondaryPressed.RemoveAllListeners();
        OnSecondaryPressed.AddListener(Attacks[weapon]);
        if(GetComponent<PlayerInventory>().SecondaryWeapon)
        {
            timeBetweenSecondaryAttack = GetComponent<PlayerInventory>().SecondaryWeapon.delayBetweenUses;
        }
    }

    public void ChangeConsumable(string consumable)
    {
        OnConsumableUsed.RemoveAllListeners();
        OnConsumableUsed.AddListener(Consumables[consumable]);
    }

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name != "Game") { return; }
        if (canAttack)
        {
            Attacking |= context.ReadValueAsButton();
        }
    }

    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name != "Game") { return; }
        if (canSecondaryAttack)
        {
            secondaryAttacking |= context.ReadValueAsButton();
        }
    }

    public void OnConsumablePressed(InputAction.CallbackContext context)
    {
        if(SceneManager.GetActiveScene().name != "Game") { return; }
        if(canUseConsumable)
        {
            usingConsumable |= context.ReadValueAsButton();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Game") { return; }

        if (Attacking && isOwned)
        {
            CmdAttack();
        }
        if(secondaryAttacking && isOwned)
        {
            CmdSecondaryAttack();
        }
        if(usingConsumable && isOwned)
        {
            CmdConsumable();
        }

        if(isOwned)
        {
            if(Inventory.Instance.GetClosestSlot(GetComponent<PlayerInventory>().Ammo))
            {
                CmdSetAmmo(Inventory.Instance.GetClosestSlot(GetComponent<PlayerInventory>().Ammo).currentInSlot);
            }
            if(Inventory.Instance.GetClosestSlot(GetComponent<PlayerInventory>().Consumable))
            {
                CmdSetConsumables(Inventory.Instance.GetClosestSlot(GetComponent<PlayerInventory>().Consumable).currentInSlot);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdAttack()
    {
        ServerAttack();
    }

    [Server]
    public void ServerAttack()
    {
        if(isServer)
        {
            RpcAttack();
        }
    }

    [ClientRpc] 
    public void RpcAttack()
    {
        this.primary = true;
        OnPrimaryPressed.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void CmdSecondaryAttack()
    {
        ServerSecondaryAttack();
    }

    [Server]
    public void ServerSecondaryAttack()
    {
        if(isServer)
        {
            RpcSecondaryAttack();
        }
    }

    [ClientRpc] 
    public void RpcSecondaryAttack()
    {
        this.primary = false;
        OnSecondaryPressed.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void CmdConsumable()
    {
        ServerConsumable();
    }

    [Server]
    public void ServerConsumable()
    {
        if(isServer)
        {
            RpcConsumable();
        }
    }

    [ClientRpc]
    public void RpcConsumable()
    {
        OnConsumableUsed.Invoke();
    }


    public void meleeAttack()
    {
        anim.SetTrigger("Attack1");
        ChangeAttack(this.primary);
    }

    public void SmallHealth()
    {
        if(canUseConsumable && ConsumableAmount > 0)
        {
            GetComponent<Health>().Heal(25);
            ChangeConsumableUse();
            Inventory.Instance.UseItem(GetComponent<PlayerInventory>().Consumable);
        }
        usingConsumable = false;
    }

    public void nullAttack()
    {
        ChangeAttack(this.primary);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetAmmo(int ammo)
    {
        SetAmmo(Ammo, ammo);
    }

    public void SetAmmo(int oldValue, int newValue)
    {
        Ammo = newValue;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetConsumables(int consumables)
    {
        SetConsumables(ConsumableAmount, consumables);
    }

    public void SetConsumables(int oldValue, int newValue)
    {
        ConsumableAmount = newValue;
    }

    public void rangedAttack()
    {
        if(this.primary)
        {
            if (!canAttack) { return; }
        }else
        {
            if(!canSecondaryAttack) { return; }
        }
        if(Ammo > 0)
        {
            if(GetComponent<PlayerInventory>().Ammo)
            {
                GameObject spawnedProjectile = Instantiate(GetComponent<PlayerInventory>().Ammo.projectile, firepoint.transform.position, firepoint.transform.rotation);
                if(isOwned)
                {
                    Inventory.Instance.UseItem(GetComponent<PlayerInventory>().Ammo);
                }
            }
        }
        ChangeAttack(this.primary);
    }

    public void shotgunAttack()
    {
        if (this.primary)
        {
            if (!canAttack) { return; }
        }
        else
        {
            if (!canSecondaryAttack) { return; }
        }
        if (Ammo > 0)
        {
            if (GetComponent<PlayerInventory>().Ammo)
            {
                for(int i = 0; i < 15; i++)
                {
                    var spread = firepoint.transform.rotation;
                    spread.x += UnityEngine.Random.Range(-0.5f, 0.5f);
                    spread.y += UnityEngine.Random.Range(-0.5f, 0.5f);
                    GameObject spawnedProjectile = Instantiate(GetComponent<PlayerInventory>().Ammo.projectile, firepoint.transform.position, spread);
                    spawnedProjectile.GetComponent<Projectile>().damage = spawnedProjectile.GetComponent<Projectile>().damage / 5;
                    spawnedProjectile.GetComponent<Projectile>().speed = spawnedProjectile.GetComponent<Projectile>().speed * 1.5f;
                }
                if (isOwned)
                {
                    Inventory.Instance.UseItem(GetComponent<PlayerInventory>().Ammo);
                }
            }
        }
        ChangeAttack(this.primary);
    }

    public void ChangeAttack(bool primary)
    {
        if(primary)
        {
            Attacking = false;
            canAttack = false;
            Invoke("ResetAttack", timeBetweenAttack);
        }
        else
        {
            secondaryAttacking = false;
            canSecondaryAttack = false;
            Invoke("ResetSecondaryAttack", timeBetweenSecondaryAttack);
        }
    }

    public void ChangeConsumableUse()
    {
        canUseConsumable = false;
        usingConsumable = false;
        Invoke(nameof(ResetConsumable), timeBetweenConsumableUse);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void ResetSecondaryAttack()
    {
        canSecondaryAttack = true;
    }

    void ResetConsumable()
    {
        canUseConsumable = true;
    }
}

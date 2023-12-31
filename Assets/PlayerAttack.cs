using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Mirror;
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

    public Animator anim;
    bool alreadyAdded = false;
    public int attackIndex = 0;
    private int lastAttack = 0;
    public int secondaryAttackIndex = 0;
    bool secondaryAlreadyAdded = false;
    private string[] lightMeleeAttack = { "Attack1", "Attack2", "Attack3" };

    bool inSwing = false;

    public bool canAttack = true;
    bool canSecondaryAttack = true;

    public bool canUseConsumable = true;

    public float timeBetweenAttack = 1f;
    public float timeBetweenSecondaryAttack = 1f;

    public float timeBetweenConsumableUse = 1f;

    public GameObject[] projectiles;
    public Transform firepoint;

    bool primary;

    public GameObject shield;
    public Transform weaponHolder;
    GameObject spawnedShield;

    PlayerInventory playerInventory;

    public Dictionary<string, UnityAction> Attacks = new Dictionary<string, UnityAction>();
    public Dictionary<string, UnityAction> Consumables = new Dictionary<string, UnityAction>();

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        Attacks.Add("", nullAttack);
        Attacks.Add("Sword", meleeAttack);
        Attacks.Add("Bow", rangedAttack);
        Attacks.Add("Shotgun", shotgunAttack);
        Attacks.Add("Shield", Shield);

        Consumables.Add("", nullAttack);
        Consumables.Add("SmallHealth", SmallHealth);

        OnPrimaryPressed.AddListener(Attacks[""]);
        OnSecondaryPressed.AddListener(Attacks[""]);
    }

    public void ChangePrimaryAttack(string weapon)
    {
        OnPrimaryPressed.RemoveAllListeners();
        OnPrimaryPressed.AddListener(Attacks[weapon]);
        if (playerInventory.PrimaryWeapon)
        {
            timeBetweenAttack = playerInventory.PrimaryWeapon.delayBetweenUses;
        }
    }

    public void ChangeSecondaryAttack(string weapon)
    {
        OnSecondaryPressed.RemoveAllListeners();
        OnSecondaryPressed.AddListener(Attacks[weapon]);
        if(playerInventory.SecondaryWeapon)
        {
            timeBetweenSecondaryAttack = playerInventory.SecondaryWeapon.delayBetweenUses;
        }
    }

    public void ChangeConsumable(string consumable)
    {
        OnConsumableUsed.RemoveAllListeners();
        OnConsumableUsed.AddListener(Consumables[consumable]);
    }

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (!Manager.Instance.settings.isPlayable) { return; }
        if(!Manager.Instance.AllowOtherInput) { return; }
        if (context.ReadValueAsButton())
        {
            if(inSwing && playerInventory.PrimaryWeapon && playerInventory.PrimaryWeapon.itemName == "Sword")
            {
                    if(attackIndex + 1 == lastAttack + 1)
                    {
                        attackIndex++;
                        Attacking |= context.ReadValueAsButton();
                    }
                    if (attackIndex > lightMeleeAttack.Length - 1)
                    {
                        attackIndex = 0;
                    }
            }else
            {
                attackIndex = 0;
                if (canAttack)
                {
                    if(spawnedShield) { return; }
                    Attacking |= context.ReadValueAsButton();
                    return;
                }
            }
        }else if (spawnedShield && playerInventory.PrimaryWeapon.itemName == "Shield")
        {
            CmdDestroyShield();
        }
    }

    public void changeSwing(bool swing)
    {
        if(attackIndex < lightMeleeAttack.Length && attackIndex != 0) { return; }
        inSwing = swing;
        if(secondaryAttackIndex < lightMeleeAttack.Length && secondaryAttackIndex != 0) { return; }
        inSwing = swing;
    }

    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        if (!Manager.Instance.settings.isPlayable) { return; }
        if (!Manager.Instance.AllowOtherInput) { return; }
        if (context.ReadValueAsButton())
        {
            if (inSwing && playerInventory.SecondaryWeapon && playerInventory.SecondaryWeapon.itemName == "Sword")
            {
                if (secondaryAttackIndex + 1 == lastAttack + 1)
                {
                    secondaryAttackIndex++;
                    secondaryAttacking |= context.ReadValueAsButton();
                }
                if (secondaryAttackIndex > lightMeleeAttack.Length - 1)
                {
                    secondaryAttackIndex = 0;
                }
            }
            else
            {
                secondaryAttackIndex = 0;
                if (canAttack)
                {
                    if (spawnedShield) { return; }
                    secondaryAttacking |= context.ReadValueAsButton();
                    return;
                }
            }
        }
        else if (spawnedShield && playerInventory.SecondaryWeapon.itemName == "Shield")
        {
            CmdDestroyShield();
        }
    }

    public void OnConsumablePressed(InputAction.CallbackContext context)
    {
        if(!Manager.Instance.settings.isPlayable) { return; }
        if (!Manager.Instance.AllowOtherInput) { return; }
        if (canUseConsumable)
        {
            usingConsumable |= context.ReadValueAsButton();
        }
    }

    private void Update()
    {
        if (!Manager.Instance.settings.isPlayable) { return; }

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
        lastAttack = attackIndex;
        anim.SetBool(lightMeleeAttack[attackIndex], true);
        ChangeAttack(this.primary);
    }

    public void resetAnimValues()
    {
        for(int i = 0; i < lightMeleeAttack.Length; i++)
        {
            if(attackIndex < lightMeleeAttack.Length && attackIndex > 0)
            {
                if(i < lightMeleeAttack.Length - 1)
                {
                    if (anim.GetBool(lightMeleeAttack[i + 1]))
                    {
                        return;
                    }else
                    {
                        foreach (var item in lightMeleeAttack)
                        {
                            anim.SetBool(item, false);
                        }
                    }
                }
            }
        }
        foreach (var item in lightMeleeAttack)
        {
            anim.SetBool(item, false);
        }
    }

    public void closeAllAnims()
    {
        foreach (var item in lightMeleeAttack)
        {
            anim.SetBool(item, false);
        }
    }

    public void Shield()
    {
        if(spawnedShield) { return; }
        spawnedShield = Instantiate(shield, weaponHolder.transform.position, transform.rotation);
        spawnedShield.GetComponent<ShieldFollow>().followTransform = weaponHolder;
    }

    [Command(requiresAuthority = false)]
    public void CmdDestroyShield()
    {
        ServerDestroyShield();
    }

    [Server]
    public void ServerDestroyShield()
    {
        RpcDestroyShield();
    }

    [ClientRpc]
    public void RpcDestroyShield()
    {
        Destroy(spawnedShield);
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
            Invoke(nameof(ResetAttack), timeBetweenAttack);
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

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

    public UnityEvent OnPrimaryPressed;
    public UnityEvent OnSecondaryPressed;

    Animator anim;

    bool canAttack = true;
    bool canSecondaryAttack = true;

    public float timeBetweenAttack = 1f;
    public float timeBetweenSecondaryAttack = 1f;

    public GameObject projectile;
    public Transform firepoint;

    bool primary;

    public Dictionary<string, UnityAction> Attacks = new Dictionary<string, UnityAction>();

    private void Awake()
    {
        anim = GetComponent<Animator>();
        Attacks.Add("", nullAttack);
        Attacks.Add("Sword", meleeAttack);
        Attacks.Add("Bow", rangedAttack);
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


    public void meleeAttack()
    {
        anim.SetTrigger("Attack1");
        ChangeAttack(this.primary);
    }

    public void nullAttack()
    {
        ChangeAttack(this.primary);
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
        if(!isOwned) { return; }
        if(Inventory.Instance.GetClosestSlot(GetComponent<PlayerInventory>().Ammo))
        {
            GameObject spawnedProjectile = null;
            spawnedProjectile = Instantiate(GetComponent<PlayerInventory>().Ammo.projectile, firepoint.transform.position, firepoint.transform.rotation);
            Inventory.Instance.UseItem(GetComponent<PlayerInventory>().Ammo);
            ChangeAttack(this.primary);
        }
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

    void ResetAttack()
    {
        canAttack = true;
    }

    void ResetSecondaryAttack()
    {
        canSecondaryAttack = true;
    }
}

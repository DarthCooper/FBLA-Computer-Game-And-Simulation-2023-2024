using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        anim = GetComponent<Animator>();
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

    public void rangedAttack()
    {
        if(isOwned)
        {
            SpawnBullet();
        }
        ChangeAttack(this.primary);
    }

    [Command(requiresAuthority = false)]
    public void SpawnBullet()
    {
        GameObject bullet = Instantiate(projectile, firepoint.transform.position, firepoint.transform.rotation);
        NetworkServer.Spawn(bullet);
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

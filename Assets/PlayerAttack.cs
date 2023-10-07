using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerAttack : NetworkBehaviour
{
    [SyncVar(hook = "UpdateAttacking")] public bool Attacking;

    public UnityEvent OnPrimaryPressed;

    Animator anim;

    bool canAttack = true;

    public float timeBetweenAttack = 1f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if(canAttack)
        {
            CmdSetAttacking(context.ReadValueAsButton());
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetAttacking(bool context)
    {
        UpdateAttacking(this.Attacking, context);
    }

    public void UpdateAttacking(bool OldValue, bool NewValue)
    {
        if(isServer)
        {
            Attacking |= NewValue;
        }
        if(isClient)
        {
            Attacking |= NewValue;
        }
    }

    private void Update()
    {
        if (Attacking)
        {
            OnPrimaryPressed.Invoke();
        }
    }

    public void meleeAttack()
    {
        anim.SetTrigger("Attack1");
        Attacking = false;
        canAttack = false;
        Invoke("ResetAttack", timeBetweenAttack);
    }

    void ResetAttack()
    {
        canAttack = true;
    }
}

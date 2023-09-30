using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public bool Attacking;

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
            Attacking |= context.ReadValueAsButton();
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

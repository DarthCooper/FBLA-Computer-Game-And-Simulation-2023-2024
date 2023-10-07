using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour
{
    PlayerMovementController playerMovementController;
    EnemyAI enemyAI;
    Rigidbody2D rb;

    [SyncVar(hook = "HealthUpdate")] public float health = 100f;

    public void TakeDamage(float damage)
    {
        CmdSetHealth(this.health - damage);

    }

    [Command(requiresAuthority = false)]
    private void CmdSetHealth(float health)
    {
        HealthUpdate(this.health, health);
    }

    public void HealthUpdate(float OldValue, float NewValue)
    {
        if (isServer)
        {
            this.health = NewValue;
        }
        if (isClient)
        {
            this.health = NewValue;
            checkIfDead();
        }
    }

    void checkIfDead()
    {
        if (health <= 0)
        {
            //die
            gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        enemyAI = GetComponent<EnemyAI>();  
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeKnockback(float time, Vector2 Direction)
    {
        if(playerMovementController != null)
        {
            playerMovementController.disableMovement(time);
        }
        if(enemyAI != null)
        {
            enemyAI.enabled = false;
            Invoke("ReEnableEnemyBrain", time);
        }
        Invoke("EndKnockback", time);
        rb.AddForce(Direction);
    }

    void ReEnableEnemyBrain()
    {
        enemyAI.enabled = true;
    }

    void EndKnockback()
    {
        rb.velocity = Vector2.zero;
    }
}
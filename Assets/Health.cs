using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Health : NetworkBehaviour
{
    PlayerMovementController playerMovementController;
    EnemyAI enemyAI;
    Rigidbody2D rb;
    ReviveMarker marker;

    public bool canBeRevived = false;
    public GameObject reviveMarker;

    [SyncVar(hook = "HealthUpdate")] public float health = 100f;

    public void TakeDamage(float damage)
    {
        CmdSetHealth(this.health - damage);
    }

    public void Heal(float healAmount)
    {
        CmdSetHealth(this.health + healAmount);
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
        if (health <= 0 && !canBeRevived)
        {
            gameObject.SetActive(false);
        }else if(canBeRevived && health <= 0 && !marker)
        {
            if(isOwned)
            {
                marker = Instantiate(reviveMarker, transform.position, Quaternion.identity).GetComponent<ReviveMarker>();
                NetworkServer.Spawn(marker.gameObject, this.gameObject);
            }
            if(marker)
            {
                marker.player = this.gameObject;
            }
            gameObject.SetActive(false);
        }

        if(health > 0 && canBeRevived && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            if(marker != null)
            {
                Destroy(marker.gameObject);
            }
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

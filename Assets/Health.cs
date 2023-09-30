using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    PlayerMovementController playerMovementController;
    EnemyAI enemyAI;
    Rigidbody2D rb;

    public float health = 100f;

    public void TakeDamage(float damage)
    {
        health -= damage;
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

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
    [HideInInspector] public float maxHealth = 100f;

    bool needsToSetHealth;
    float savedHealth;

    public GameObject[] itemsToSpawn;

    public void TakeDamage(float damage)
    {
        CmdSetHealth(this.health - damage);
    }

    public void SetHealth(float health)
    {
        needsToSetHealth = true;
        savedHealth = health;
        CmdSaveSetHealth(savedHealth);
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

    [Command(requiresAuthority = false)]
    private void CmdSaveSetHealth(float health)
    {
        needsToSetHealth = false;
        HealthUpdate(this.health, health);
    }

    private void Update()
    {
        if (needsToSetHealth)
        {
            CmdSaveSetHealth(savedHealth);
        }
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

    [Command(requiresAuthority = false)]
    public void CmdSpawnItems()
    {
        foreach (var item in itemsToSpawn)
        {
            var Item = Instantiate(item, transform.position, transform.rotation);
            NetworkServer.Spawn(Item);
        }
    }

    void checkIfDead()
    {
        if (health <= 0 && itemsToSpawn.Length > 0 && gameObject.activeSelf)
        {
            CmdSpawnItems();
        }
        if (health <= 0 && !canBeRevived)
        {
            gameObject.SetActive(false);
            Manager.Instance.miscEvents.EnemyKilled();
        }else if(canBeRevived && health <= 0 && !marker)
        {
            if(isOwned)
            {
                CmdSpawnGrave();
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

    [Command]
    public void CmdSpawnGrave()
    {
        marker = Instantiate(reviveMarker, transform.position, Quaternion.identity).GetComponent<ReviveMarker>();
        NetworkServer.Spawn(marker.gameObject, this.gameObject);
        marker.player = this.gameObject;
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

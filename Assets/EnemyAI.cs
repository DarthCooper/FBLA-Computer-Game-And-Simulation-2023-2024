using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;
using Mirror;
using UnityEngine.UIElements;

public class EnemyAI : NetworkBehaviour
{
    public string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public Transform target;
    public float targetTolerance;

    public float waypointTolerance;

    public EnemyStates currentState = EnemyStates.Patroling;

    public UnityEvent OnReachedTarget;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    public Path path;
    public int currentWaypoint = 0;
    public bool reachedEndOfPath = false;

    public bool runIfPlayerIsClose;
    public float runDistance;
    public float DistanceFromPlayer;
    public LayerMask runLayer;
    public Transform tempTarget;
    public Transform tempTargetPrefab;

    public Seeker seeker;
    public Rigidbody2D rb;

    public Animator animator;

    public bool canMove = true;

    public float timeBetweenAttacks;
    bool canAttack = true;
    public float timeSinceLastAttack = 0;

    public bool waitingForReEnable;

    public GameObject projectile;
    public Transform Firepoint;
    public Transform FirepointPivot;

    public LayerMask targetLayer;

    public bool stunned;

    public bool facingLeft = true;

    public Transform graphics;

    public GameObject healParticles;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponentInChildren<Animator>();

        if(!isServer) { return; }
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    private void Awake()
    {
        //tempTarget = Instantiate(tempTargetPrefab, transform.position, transform.rotation);
    }

    void UpdatePath()
    {
        if(target == null) {  return; }
        if(seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        onUpdate();
    }

    public virtual void onUpdate()
    {
        if (Firepoint && GetComponent<GetClosestTarget>().Target != null)
        {
            Vector3 offset = GetComponent<GetClosestTarget>().Target.transform.position - transform.position;
            FirepointPivot.rotation = Quaternion.LookRotation(Vector3.forward,offset);
        }
        if (!isServer) { return; } //movement will only be ran on the server while the clients recieve the data from the server.

        if (stunned) { return; }

        if (target == null && tempTarget != null)
        {
            target = tempTarget;
        }
        else if (target == null)
        {
            tempTarget = Instantiate(tempTargetPrefab, transform.position, transform.rotation);
            target = tempTarget;
        }
        if (GetComponent<Patrol>())
        {
            if (GetComponent<Patrol>().target == null)
            {
                GetComponent<Patrol>().target = tempTarget;
            }
        }

        if (path == null) { return; }
        if (currentWaypoint >= path.vectorPath.Count) { return; }

        if (currentState == EnemyStates.Patroling)
        {
            target = tempTarget;
        }

        float distanceToPlayer = 0;
        if (GetComponent<GetClosestTarget>().Target)
        {
            distanceToPlayer = Vector2.Distance(rb.position, GetComponent<GetClosestTarget>().Target.transform.position);
        }
        if (distanceToPlayer >= DistanceFromPlayer && (currentState == EnemyStates.Hunting || currentState == EnemyStates.Fleeing))
        {
            target = GetComponent<GetClosestTarget>().Target.transform;
            currentState = EnemyStates.Hunting;
        }
        else
        {
            if (runIfPlayerIsClose && distanceToPlayer < DistanceFromPlayer)
            {
                currentState = EnemyStates.Fleeing;
                if (tempTarget == null)
                {
                    tempTarget = Instantiate(tempTargetPrefab, transform.position, transform.rotation);
                }
                if (GetComponent<GetClosestTarget>().Target)
                {
                    RaycastHit2D NewPos = Physics2D.Raycast(transform.position, transform.position - GetComponent<GetClosestTarget>().Target.transform.position, runDistance, runLayer);
                    if (NewPos)
                    {
                        tempTarget.transform.position = NewPos.transform.position;
                        target = tempTarget;
                    }
                    else
                    {
                        Ray2D ray = new Ray2D(transform.position, transform.position - GetComponent<GetClosestTarget>().Target.transform.position);
                        tempTarget.transform.position = ray.GetPoint(runDistance);
                        target = tempTarget;
                    }
                }
            }
        }

        float distanceToTarget = Vector2.Distance(rb.position, target.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, targetTolerance, targetLayer);

        if (currentState == EnemyStates.Patroling && GetComponent<Patrol>())
        {
            if (distanceToTarget <= waypointTolerance)
            {
                GetComponent<Patrol>().FindNewTarget();
                target = tempTarget;
            }
        }

        if (distanceToTarget <= targetTolerance)
        {
            if (hit)
            {
                if (hit.collider.gameObject == target.gameObject)
                {
                    reachedEndOfPath = true;
                    canMove = false;
                }
                else
                {
                    reachedEndOfPath = false;
                    if (!waitingForReEnable)
                    {
                        canMove = true;
                    }
                }
            }
            else
            {
                reachedEndOfPath = false;
                if (!waitingForReEnable)
                {
                    canMove = true;
                }
            }
        }
        else
        {
            reachedEndOfPath = false;
            if (!waitingForReEnable)
            {
                canMove = true;
            }
        }

        if (reachedEndOfPath)
        {
            CmdReachedTarget();
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;

        Vector2 difference = target.position - transform.position;

        if (canMove)
        {
            if(difference.x > 0 && facingLeft)
            {
                graphics.localScale = new Vector3(graphics.localScale.x * -1, 1, 1);
                facingLeft = false;
            }else if(difference.x < 0 && !facingLeft)
            {
                graphics.localScale = new Vector3(graphics.localScale.x * -1, 1, 1);
                facingLeft = true;
            }
            rb.AddForce(force);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        PlayAnim(canMove);
    }

    [Command(requiresAuthority = false)]
    public void CmdReachedTarget()
    {
        ServerReachedTarget();
    }

    [Server]
    public void ServerReachedTarget()
    {
        if(isServer)
        {
            RpcReachedTarget();
        }
    }

    [ClientRpc]
    public void RpcReachedTarget()
    {
        OnReachedTarget.Invoke();
    }

    public void MeleeAttack()
    {
        if(canAttack)
        {
            animator.SetTrigger("Attack");
            canAttack = false;
            Invoke("AttackDelay", timeBetweenAttacks);
        }
    }

    public void RangedAttack()
    {
        if(canAttack)
        {
            animator.SetTrigger("Attack");
            Invoke(nameof(spawnProjectile), 0.5f);
            canAttack = false;
            Invoke("AttackDelay", timeBetweenAttacks);
        }
    }

    public void spawnProjectile()
    {
        Instantiate(projectile, Firepoint.position, Firepoint.rotation);
    }

    public void SupportAttack()
    {
        if(canAttack)
        {
            StopMovement(4f);
            print("Healed" + target.GetComponent<Health>().health);
            animator.SetTrigger("Attack");
            Health targetHealth = target.GetComponent<Health>();
            targetHealth.health += 50;
            if(healParticles)
            {
                var selfParticles = Instantiate(healParticles, this.transform.position, Quaternion.identity);
                selfParticles.transform.SetParent(transform);
                var targetParticles = Instantiate(healParticles, target.transform.position, Quaternion.identity);
                targetParticles.transform.SetParent(target);
            }
            canAttack = false;
            Invoke("AttackDelay", timeBetweenAttacks);
        }
    }

    public void AttackDelay()
    {
        canAttack = true;
    }

    public void PlayAnim(bool walking)
    {
        animator.SetBool("Walking", walking);
    }

    public void StopMovement(float time)
    {
        canMove = false;
        waitingForReEnable = true;
        Invoke("ReEnableMovement", time);
    }

    void ReEnableMovement()
    {
        canMove = true;
        waitingForReEnable = false;
    }

    public void StunEnemy(float time)
    {
        StopMovement(time);
        waitingForReEnable = true;
        stunned = true;
        Invoke(nameof(DisableStun), time);
    }

    void DisableStun()
    {
        stunned = false;
    }
}

public enum EnemyStates 
{
    Hunting,
    Patroling,
    Fleeing,
}
    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;
using Mirror;
using JetBrains.Annotations;

public class EnemyAI : NetworkBehaviour
{
    public Transform target;
    public float targetTolerance;

    public float waypointTolerance;

    public EnemyStates currentState = EnemyStates.Patroling;

    public UnityEvent OnReachedTarget;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    public bool reachedEndOfPath = false;

    public bool runIfPlayerIsClose;
    public float runDistance;
    public float DistanceFromPlayer;
    public LayerMask runLayer;
    public Transform tempTarget;
    public Transform tempTargetPrefab;

    Seeker seeker;
    Rigidbody2D rb;

    Animator animator;

    public bool canMove = true;

    public float timeBetweenAttacks;
    bool canAttack = true;
    public float timeSinceLastAttack = 0;

    bool waitingForReEnable;

    public GameObject projectile;
    public Transform Firepoint;

    public LayerMask targetLayer;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

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
        if (Firepoint && target != null)
        {
            Firepoint.up = target.position - Firepoint.position;
        }
        if (!isServer) { return; } //movement will only be ran on the server while the clients recieve the data from the server.

        if(target == null && tempTarget != null)
        {
            target = tempTarget;
        }else if(target == null)
        {
            tempTarget = Instantiate(tempTargetPrefab, transform.position, transform.rotation);
            target = tempTarget;
        }
        if(GetComponent<Patrol>().target == null)
        {
            GetComponent<Patrol>().target = tempTarget;
        }

        if (path == null) { return; }
        if (currentWaypoint >= path.vectorPath.Count) { return; }

        if(currentState == EnemyStates.Patroling)
        {
            target = tempTarget;
        }

        float distanceToPlayer = Vector2.Distance(rb.position, GetComponent<GetClosestTarget>().Target.transform.position);
        if(distanceToPlayer >= DistanceFromPlayer && (currentState == EnemyStates.Hunting || currentState == EnemyStates.Fleeing))
        {
            target = GetComponent<GetClosestTarget>().Target.transform;
            currentState = EnemyStates.Hunting;
        }else
        {
            if (runIfPlayerIsClose && distanceToPlayer < DistanceFromPlayer)
            {
                currentState = EnemyStates.Fleeing;
                if(tempTarget == null)
                {
                    tempTarget = Instantiate(tempTargetPrefab, transform.position, transform.rotation);
                }
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

        float distanceToTarget = Vector2.Distance(rb.position, target.position);
        RaycastHit2D hit =  Physics2D.Raycast(transform.position, target.position - transform.position, targetTolerance, targetLayer);

        if(currentState == EnemyStates.Patroling)
        {
            if(distanceToTarget <= waypointTolerance)
            {
                GetComponent<Patrol>().FindNewTarget();
                target = tempTarget;
            }
        }

        if(distanceToTarget <= targetTolerance)
        {
            if(hit)
            {
                if(hit.collider.gameObject == target.gameObject)
                {
                    reachedEndOfPath = true;
                    canMove = false;
                }else
                {
                    reachedEndOfPath = false;
                    if (!waitingForReEnable)
                    {
                        canMove = true;
                    }
                }
            }else
            {
                reachedEndOfPath = false;
                if (!waitingForReEnable)
                {
                    canMove = true;
                }
            }
        }else
        {
            reachedEndOfPath = false;
            if(!waitingForReEnable)
            {
                canMove=true;
            }
        }

        if(reachedEndOfPath)
        {
            CmdReachedTarget();
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;

        if(canMove)
        {
            rb.AddForce(force);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        PlayAnim((Vector2)target.transform.position - rb.position);
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
            animator.SetTrigger("Attack1");
            canAttack = false;
            Invoke("AttackDelay", timeBetweenAttacks);
        }
    }

    public void RangedAttack()
    {
        if(canAttack)
        {
            Instantiate(projectile, Firepoint.position, Firepoint.rotation);
            canAttack = false;
            Invoke("AttackDelay", timeBetweenAttacks);
        }
    }

    public void SupportAttack()
    {
        if(canAttack)
        {
            print("Healed" + target.GetComponent<Health>().health);
            target.GetComponent<Health>().health += 25;
            canAttack = false;
            Invoke("AttackDelay", timeBetweenAttacks);
        }
    }

    public void AttackDelay()
    {
        canAttack = true;
    }

    public void PlayAnim(Vector2 diff)
    {
        animator.SetFloat("X", diff.x);
        animator.SetFloat("Y", diff.y);
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
}

public enum EnemyStates 
{
    Hunting,
    Patroling,
    Fleeing,
}
    
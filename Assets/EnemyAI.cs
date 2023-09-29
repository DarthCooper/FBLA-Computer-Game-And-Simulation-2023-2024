using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float targetTolerance;

    public UnityEvent OnReachedTarget;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    public bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    Animator animator;

    public bool canMove = true;

    public float timeBetweenAttacks;
    bool canAttack = true;
    public float timeSinceLastAttack = 0;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        InvokeRepeating("UpdatePath", 0f, .5f);
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
        if (path == null) { return; }

        if (currentWaypoint >= path.vectorPath.Count) { return; }

        float distanceToTarget = Vector2.Distance(rb.position, target.position);
        if(distanceToTarget <= targetTolerance)
        {
            reachedEndOfPath = true;
            canMove = false;
        }else
        {
            reachedEndOfPath = false;
            canMove=true;
        }

        if(reachedEndOfPath)
        {
            OnReachedTarget.Invoke();
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

    public void MeleeAttack()
    {
        if(canAttack)
        {
            animator.SetTrigger("Attack1");
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
        Invoke("ReEnableMovement", time);
    }

    void ReEnableMovement()
    {
        canMove = true;
    }
}
    
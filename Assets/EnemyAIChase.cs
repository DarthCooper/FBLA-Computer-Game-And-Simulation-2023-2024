using Mirror;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAIChase : EnemyAI
{

    private void Awake()
    {
        currentState = EnemyStates.Hunting;
    }

    public override void onUpdate()
    {
        if (Firepoint && target != null)
        {
            Firepoint.up = target.position - Firepoint.position;
        }
        if (!isServer) { return; } //movement will only be ran on the server while the clients recieve the data from the server.

        if (stunned) { return; }
        if (path == null) { return; }
        if (currentWaypoint >= path.vectorPath.Count) { return; }

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

        if (canMove)
        {
            rb.AddForce(force);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        PlayAnim((Vector2)target.transform.position - rb.position);
    }
}

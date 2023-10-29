using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    EnemyAI ai;

    public Transform target;

    public Collider2D HuntingSight;
    public Collider2D PatrollingSight;

    private void Awake()
    {
        ai = GetComponentInParent<EnemyAI>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var tag in ai.GetComponent<GetClosestTarget>().TargetTags)
        {
            if(collision.gameObject.tag == tag)
            {
                target = collision.gameObject.transform;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        foreach (var tag in ai.GetComponent<GetClosestTarget>().TargetTags)
        {
            if (collision.gameObject.tag == tag)
            {
                target = null;
            }
        }
    }

    public bool canSeeTarget()
    {
        if(target == null) { return false; }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, Vector3.Distance(transform.position, target.position), ai.targetLayer);
        foreach(var tag in ai.GetComponent<GetClosestTarget>().TargetTags)
        {
            if(hit)
            {
                if (hit.collider.tag == tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Update()
    {
        if(canSeeTarget() && ai.currentState != EnemyStates.Fleeing && ai.currentState != EnemyStates.Hunting)
        {
            ai.currentState = EnemyStates.Hunting;
            HuntingSight.enabled = true;
            PatrollingSight.enabled = false;
        }else if(ai.currentState != EnemyStates.Fleeing && target == null)
        {
            ai.currentState = EnemyStates.Patroling;
            HuntingSight.enabled = false;
            PatrollingSight.enabled = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    EnemyAI ai;

    public List<Transform> targets;

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
                targets.Add(collision.gameObject.transform);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        foreach (var tag in ai.GetComponent<GetClosestTarget>().TargetTags)
        {
            if (collision.gameObject.tag == tag)
            {
                targets.Remove(collision.gameObject.transform);
            }
        }
    }

    public bool canSeeTarget()
    {
        foreach(var target in targets)
        {
            if(target == null) { return false; }
            RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, Vector3.Distance(transform.position, target.position) + 2f, ai.targetLayer);
            string[] targetTags = new string[0];
            if(ai.GetComponent<GetClosestEnemy>())
            {
                targetTags = ai.GetComponent<GetClosestEnemy>().TargetTags;
            }
            else if (ai.GetComponent<GetClosestTarget>())
            {
                targetTags = ai.GetComponent<GetClosestTarget>().TargetTags;
            }
            foreach(var tag in targetTags)
            {
                if(hit)
                {
                    print(transform.parent.name + ":"  + hit.collider.tag == tag);
                    if (hit.collider.tag == tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void Update()
    {
        if(canSeeTarget() && ai.currentState != EnemyStates.Fleeing)
        {
            ai.currentState = EnemyStates.Hunting;
            HuntingSight.enabled = true;
            PatrollingSight.enabled = false;
        }else if(!canSeeTarget() && ai.currentState != EnemyStates.Fleeing && targets.Count <= 0)
        {
            ai.currentState = EnemyStates.Patroling;
            HuntingSight.enabled = false;
            PatrollingSight.enabled = true;
        }
    }
}

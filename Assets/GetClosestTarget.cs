using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetClosestTarget : MonoBehaviour
{
    public string[] TargetTags;
    public GameObject Target;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetClosest", 0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        SetTarget();
    }

    public virtual void SetTarget()
    {
        if (Target == null) { return; }
        if (GetComponent<EnemyAI>().currentState != EnemyStates.Patroling && GetComponent<EnemyAI>().currentState != EnemyStates.Fleeing)
        {
            GetComponent<EnemyAI>().target = Target.transform;
        }
    }

    public virtual void GetClosest()
    {
        float maxDistance = 10000;
        foreach (var tag in TargetTags)
        {
            foreach (var target in GameObject.FindGameObjectsWithTag(tag))
            {
                float Distance = Vector2.Distance(transform.position, target.transform.position);
                if(Distance < maxDistance)
                {
                    maxDistance = Distance;
                    Target = target;
                }
            }
        }
    }
}

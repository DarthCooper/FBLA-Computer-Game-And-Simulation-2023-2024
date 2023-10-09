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
        if (GetComponent<EnemyAI>().target != Target.transform && (GetComponent<EnemyAI>().tempTarget == null || GetComponent<EnemyAI>().target != GetComponent<EnemyAI>().tempTarget))
        {
            GetComponent<EnemyAI>().CmdSetTarget(Target.transform);
        }
    }

    public virtual void GetClosest()
    {
        float maxDistance = 0;
        foreach (var tag in TargetTags)
        {
            foreach (var target in GameObject.FindGameObjectsWithTag(tag))
            {
                float Distance = Vector2.Distance(transform.position, target.transform.position);
                if(Distance > maxDistance)
                {
                    maxDistance = Distance;
                    Target = target;
                }
            }
        }
    }
}

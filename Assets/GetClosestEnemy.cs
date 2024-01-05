using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetClosestEnemy : GetClosestTarget
{
    void Update()
    {
        SetTarget();
    }

    public override void GetClosest()
    {
        float maxDistance = 0;
        float previousHealth = Mathf.Infinity;
        foreach (var tag in TargetTags)
        {
            foreach (var target in GameObject.FindGameObjectsWithTag(tag))
            {
                float Distance = Vector2.Distance(transform.position, target.transform.position);
                if(!target.GetComponent<Health>()) { continue; }
                if (Distance > maxDistance && target.GetComponent<Health>().health < previousHealth && target != this.gameObject)
                {
                    previousHealth = target.GetComponent<Health>().health;
                    maxDistance = Distance;
                    Target = target;
                }
            }
        }
    }
}

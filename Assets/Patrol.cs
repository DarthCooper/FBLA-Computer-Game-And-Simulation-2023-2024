using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Transform target;

    public float searchRadius;

    public LayerMask targetLayers;

    public void FindNewTarget()
    {
        bool positionWorks = false;
        Vector3 position = Vector3.zero;
        int loops = 0;
        while(!positionWorks) 
        { 
            loops++;
            position = new Vector3(Random.Range(-searchRadius, searchRadius) + this.transform.position.x, Random.Range(-searchRadius, searchRadius) + this.transform.position.y, 0);
            float distance = Vector3.Distance(this.transform.position, position);
            RaycastHit2D ray = Physics2D.Raycast(this.transform.position, position - transform.position, distance, targetLayers);
            if(!ray)
            {
                positionWorks = true;
            }
        }
        target.position = position;
    }
}

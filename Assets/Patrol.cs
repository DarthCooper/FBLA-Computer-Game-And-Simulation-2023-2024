using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Transform target;

    public float searchRadius;

    int maxloops = 25;

    public List<Vector3> positions = new List<Vector3>();

    public LayerMask targetLayers;

    public void FindNewTarget()
    {
        print("Called");
        bool positionWorks = false;
        Vector3 position = Vector3.zero;
        int loops = 0;
        while(!positionWorks && loops < maxloops) 
        { 
            loops++;
            position = new Vector3(Random.Range(-searchRadius, searchRadius), Random.Range(-searchRadius, searchRadius), 0);
            float distance = Vector3.Distance(this.transform.position, position);
            RaycastHit2D ray = Physics2D.Raycast(this.transform.position, position - transform.position, distance, targetLayers);
            if(!ray)
            {
                positionWorks = true;
                positions.Add(position);
            }
        }
        if(!positionWorks)
        {
            position = positions[0];
            positions.RemoveAt(0);
        }
        target.position = position;
    }
}

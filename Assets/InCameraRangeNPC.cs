using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InCameraRangeNPC : NPC
{
    bool stepsStarted;
    void Update()
    {
        move();
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x >= 0 && viewPos.x <= 0.75 && viewPos.y >= 0 && viewPos.y <= 0.75 && viewPos.z > 0 && !stepsStarted)
        {
            ExecuteStep();
            stepsStarted = true;
        }
    }
}

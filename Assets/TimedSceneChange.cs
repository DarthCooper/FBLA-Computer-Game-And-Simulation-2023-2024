using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSceneChange : SceneChange
{
    public float timeTillChange = 120f;

    // Update is called once per frame
    void Update()
    {
        timeTillChange -= Time.deltaTime;
        if(timeTillChange < 0)
        {
            ChangeScene(newSceneNames[0]);
        }
    }
}

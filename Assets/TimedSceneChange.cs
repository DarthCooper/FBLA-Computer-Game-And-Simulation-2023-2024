using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSceneChange : MonoBehaviour
{
    public float timeTillChange = 120f;
    public string newSceneName;
    bool changedScene;

    // Update is called once per frame
    void Update()
    {
        timeTillChange -= Time.deltaTime;
        if(timeTillChange < 0 && !changedScene)
        {
            changedScene = true;
            GetComponent<SceneSettings>().ChangeScene(newSceneName);
        }
    }
}

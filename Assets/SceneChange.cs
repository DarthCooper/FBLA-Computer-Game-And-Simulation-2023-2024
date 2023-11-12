using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    public string[] newSceneNames;
    bool changedScene = false;

    public void ChangeScene(string sceneName)
    {
        if(!changedScene)
        {
            GameObject.Find("LocalGamePlayer").transform.position = Vector3.zero;
            DataPersistenceManager.instance.SaveGame();
            changedScene = true;
            GetComponent<SceneSettings>().ChangeScene(sceneName);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    public string[] newSceneNames;
    bool changedScene = false;

    public void ChangeScene(string sceneName, float xPos = 0, float yPos = 0)
    {
        if(!changedScene)
        {
            Vector3 pos = new Vector3 (xPos, yPos);
            GameObject.Find("LocalGamePlayer").transform.position = pos;
            Manager.Instance.SetPlayerPos(xPos, yPos);
            DataPersistenceManager.instance.SaveGame();
            changedScene = true;
            GetComponent<SceneSettings>().ChangeScene(sceneName);
        }
    }
}

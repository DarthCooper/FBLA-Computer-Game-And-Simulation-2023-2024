using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettings : MonoBehaviour
{
    public bool isPlayable;
    public bool isSavable;

    public bool Minigame;

    public SceneChange sceneChanger;

    private void Awake()
    {
        sceneChanger = GetComponent<SceneChange>();
        if(Manager.Instance)
        {
            if(Manager.Instance.animator)
            {
                Manager.Instance.animator.SetTrigger("ToClear");
            }
        }
    }

    private void Update()
    {
        if(!sceneChanger)
        {
            sceneChanger = GetComponent<SceneChange>();
        }
    }

    public void ChangeScene(string name)
    {
       Manager.Instance.LoadNewLevel(name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettings : MonoBehaviour
{
    public bool isPlayable;
    public bool isSavable;

    public SceneChange sceneChanger;

    private void Awake()
    {
        sceneChanger = GetComponent<SceneChange>();
    }

    public void ChangeScene(string name)
    {
       Manager.Instance.LoadNewLevel(name);
    }
}
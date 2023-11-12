using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInteractable : MonoBehaviour
{
    public string newScene;

    public void ChangeScene()
    {
        Manager.Instance.settings.sceneChanger.ChangeScene(newScene);
    }
}

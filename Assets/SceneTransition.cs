using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public void ChangeScene()
    {
        Manager.Instance.changeScene();
    }
}

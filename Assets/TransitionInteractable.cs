using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInteractable : MonoBehaviour
{
    public string newScene;

    public Vector3 exitPos;

    public string[] questIdsToFinish;

    public void ChangeScene()
    {
        Manager.Instance.miscEvents.NpcPositionReached();
        Manager.Instance.settings.sceneChanger.ChangeScene(newScene, exitPos.x, exitPos.y);
        DataPersistenceManager.instance.SaveGame();
    }
}

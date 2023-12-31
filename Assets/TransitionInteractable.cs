using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInteractable : MonoBehaviour
{
    public string newScene;

    public Vector3 exitPos;

    public string[] questIdsToAdvance;

    bool changed = false;

    public void ChangeScene()
    {
        if(changed) { return; }
        foreach (var id in questIdsToAdvance)
        {
            QuestManager.instance.AdvanceQuest(id);
        }
        Manager.Instance.miscEvents.NpcPositionReached();
        Manager.Instance.settings.sceneChanger.ChangeScene(newScene, exitPos.x, exitPos.y);
        DataPersistenceManager.instance.SaveGame();
        changed = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractQuestStep : QuestStep
{
    public string interactableName;

    public Interactable interactable;

    public string setProgress;

    public void SetProgress()
    {
        progress = setProgress;
    }

    public void Update()
    {
        if (interactable == null)
        {
            Interactable[] interactables = GameObject.FindObjectsByType<Interactable>(FindObjectsSortMode.None);
            foreach (var interactable in interactables)
            {
                if (interactable.name == interactableName)
                {
                    this.interactable = interactable;
                    break;
                }
            }
        }
        if (interactable != null)
        {
            interactable.OnInteract.AddListener(OnInteract);
            SetProgress();
        }else
        {
            progress = "In another area";
        }
    }

    public void OnInteract()
    {
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        throw new System.NotImplementedException();
    }
}

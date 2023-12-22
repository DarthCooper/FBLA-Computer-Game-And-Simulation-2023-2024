using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWaitForInteraction : NPCStep
{
    public string interactableName;

    public Interactable interactable;

    public void Update()
    {
        if(interactable == null)
        {
            Interactable[] interactables = GameObject.FindObjectsByType<Interactable>(FindObjectsSortMode.None);
            foreach(var interactable in interactables)
            {
                if(interactable.name == interactableName)
                {
                    this.interactable = interactable;
                    break;
                }
            }
        }
        if(interactable != null)
        {
            interactable.OnInteract.AddListener(OnInteract);
        }
    }

    public void OnInteract()
    {
        npc.EndStep();
    }

}

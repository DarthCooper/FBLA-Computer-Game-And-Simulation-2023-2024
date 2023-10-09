using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerInteract : NetworkBehaviour
{
    public bool interact;

    public Interactable interactable;
    public void OnInteract(InputAction.CallbackContext context)
    {
        interact = context.ReadValueAsButton();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Interactable>() != null)
        {
            interactable = collision.gameObject.GetComponent<Interactable>();
        }
    }

    private void Update()
    {
        if (!isOwned)
        {
            return;
        }
        if (interact && interactable)
        {
            if(!interactable.beenInteractedWith)
            {
                interactable.Interact();
            }
        }else if(!interact && interactable)
        {
            interactable.EndInteract();
        }
    }
}

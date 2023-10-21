using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerInteract : NetworkBehaviour
{
    public bool interact;

    public bool InventoryOpen = false;
    public GameObject inventory;
    bool canOpenInventory = false;

    public Interactable interactable;

    public void OnInteract(InputAction.CallbackContext context)
    {
        interact = context.ReadValueAsButton();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton() && !canOpenInventory)
        {
            InventoryOpen = !InventoryOpen;
        }
        canOpenInventory = context.ReadValueAsButton();
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
        if(!inventory && SceneManager.GetActiveScene().name == "Game")
        {
            inventory = GameObject.Find("Canvas");
            if(inventory)
            {
                inventory = inventory.transform.Find("Menu").gameObject;
                if(inventory)
                {
                    inventory = inventory.transform.Find("PlayerMenu").gameObject;
                }
            }
        }
        if (interact && interactable)
        {
            if(!interactable.beenInteractedWith)
            {
                interactable.Interact(this.gameObject);
            }
        }else if(!interact && interactable)
        {
            interactable.EndInteract();
        }
        if(inventory)
        {
            if(inventory.name != "PlayerMenu") { return; }
            if (InventoryOpen != inventory.activeSelf)
            {
                inventory.SetActive(InventoryOpen);
            }
        }
    }
}

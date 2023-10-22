using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using TMPro;

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

    public void CheckInventory()
    {
        while (inventory == null || inventory.name != "PlayerMenu")
        {
            SetInventory();
        }
        foreach (var image in inventory.GetComponentsInChildren<Image>())
        {
            if (!image.gameObject.GetComponentInParent<SlotOptions>())
            {
                if (image.isActiveAndEnabled != InventoryOpen)
                {
                    image.enabled = InventoryOpen;
                }
            }
        }
        foreach (var image in inventory.GetComponentsInChildren<RawImage>())
        {
            if (!image.gameObject.GetComponentInParent<SlotOptions>())
            {
                if (image.isActiveAndEnabled != InventoryOpen)
                {
                    image.enabled = InventoryOpen;
                }
            }
        }
        foreach (var text in inventory.GetComponentsInChildren<TMP_Text>())
        {
            if (!text.gameObject.GetComponentInParent<SlotOptions>())
            {
                if (text.isActiveAndEnabled != InventoryOpen)
                {
                    text.enabled = InventoryOpen;
                }
            }
        }
        if(!InventoryOpen)
        {
            Inventory.Instance.ClostSlotOptions();
        }
    }

    public void SetInventory()
    {
        if (!inventory && SceneManager.GetActiveScene().name == "Game")
        {
            inventory = GameObject.Find("Canvas");
            if (inventory)
            {
                inventory = inventory.transform.Find("Menu").gameObject;
                if (inventory)
                {
                    inventory = inventory.transform.Find("PlayerMenu").gameObject;
                }
            }
        }
    }

    private void Update()
    {
        if (!isOwned)
        {
            return;
        }
        SetInventory();
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
            CheckInventory();
        }
    }
}

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

    public bool JournalOpen = false;
    public GameObject journal;
    bool canOpenJournal = false;

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

    public void OnJournal(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton() && !canOpenJournal)
        {
            JournalOpen = !JournalOpen;
        }
        canOpenJournal = context.ReadValueAsButton();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Interactable>() != null)
        {
            interactable = collision.gameObject.GetComponent<Interactable>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Interactable>() != null)
        {
            interactable = null;
        }
    }

    public void CheckInventory()
    {
        while (inventory == null || inventory.name != "PlayerMenu")
        {
            SetInventory();
        }
        EnableDisableInventory();
        if (!InventoryOpen)
        {
            Inventory.Instance.ClostSlotOptions();
        }
    }

    public void CheckJournal()
    {
        while (journal == null || journal.name != "PlayerJournal")
        {
            SetJournal();
        }
        EnableDisableJournal();
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

    public void EnableDisableInventory()
    {
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
    }

    public void EnableDisableJournal()
    {
        foreach (var image in journal.GetComponentsInChildren<Image>())
        {
            image.enabled = JournalOpen;
        }
        foreach (var image in journal.GetComponentsInChildren<RawImage>())
        {
            image.enabled = JournalOpen;
        }
        foreach (var text in journal.GetComponentsInChildren<TMP_Text>())
        {
            text.enabled = JournalOpen;
        }
    }

    public void SetJournal()
    {
        if (!journal && SceneManager.GetActiveScene().name == "Game")
        {
            journal = GameObject.Find("Canvas");
            if (journal)
            {
                journal = journal.transform.Find("Journal").gameObject;
                if (journal)
                {
                    journal = journal.transform.Find("PlayerJournal").gameObject;
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
        SetJournal();
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
        if(journal)
        {
            CheckJournal();
        }
    }
}

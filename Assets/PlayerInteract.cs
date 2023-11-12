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

    float mouse = 0;

    public bool InventoryOpen = false;
    public GameObject inventory;
    bool canOpenInventory = false;

    public bool JournalOpen = false;
    public GameObject journal;
    bool canOpenJournal = false;

    public bool reading = false;
    public GameObject readingPage;
    bool canRead = false;
    public string message;

    public List<Interactable> interactables = new List<Interactable>();
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

    public void Read(string message)
    {
        if(!canRead)
        {
            reading = !reading;
        }
        this.message = message;
        canRead = true;
    }

    public void ResetCanRead()
    {
        canRead = false;
    }

    public void OnMouseScroll(InputAction.CallbackContext context)
    {
        mouse += context.ReadValue<Vector2>().normalized.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Interactable>() != null)
        {
            interactables.Add(collision.gameObject.GetComponent<Interactable>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Interactable>() != null)
        {
            interactables.Remove(collision.gameObject.GetComponent<Interactable>());
            collision.gameObject.GetComponent<Interactable>().ChangeSelectableView(false);
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

    public void CheckReading()
    {
        while(readingPage == null ||  readingPage.name != "Sign")
        {
            SetReadingPage();
        }
        EnableDisableRead();
    }

    public void SetInventory()
    {
        if (!inventory && Manager.Instance.settings.isPlayable)
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
        if (!journal && Manager.Instance.settings.isPlayable)
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

    public void EnableDisableRead()
    {
        if(reading && readingPage)
        {
            readingPage.GetComponentInParent<ReadPage>().SetMessage(message);
        }else if(readingPage)
        {
            readingPage.GetComponentInParent<ReadPage>().SetMessage("");
        }
        foreach (var image in readingPage.GetComponentsInChildren<Image>())
        {
            image.enabled = reading;
        }
        foreach (var image in readingPage.GetComponentsInChildren<RawImage>())
        {
            image.enabled = reading;
        }
        foreach (var text in readingPage.GetComponentsInChildren<TMP_Text>())
        {
            text.enabled = reading;
        }
    }

    public void SetReadingPage()
    {
        if (!readingPage && Manager.Instance.settings.isPlayable)
        {
            readingPage = GameObject.Find("Canvas");
            if (readingPage)
            {
                readingPage = readingPage.transform.Find("Readable").gameObject;
                if (readingPage)
                {
                    readingPage = readingPage.transform.Find("Sign").gameObject;
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
        SetReadingPage();
        if(interactables.Count > 0)
        {
            interactable = interactables[Mathf.Abs((int)mouse) % interactables.Count];
            interactable.ChangeSelectableView(true);
        }
        else
        {
            interactable = null;
        }
        foreach(var interactable in interactables)
        {
            if(interactable != this.interactable)
            {
                interactable.ChangeSelectableView(false);
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
            CheckInventory();
        }
        if(journal)
        {
            CheckJournal();
        }
        if(readingPage)
        {
            CheckReading();
        }
    }
}

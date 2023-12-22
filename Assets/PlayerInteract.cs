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

    public bool speaking = false;
    public GameObject dialoguePage;
    public string speechMessage;
    public string speaker;

    public bool shopOpen = false;
    public GameObject shop;
    bool canOpenShop = false;

    public bool PauseMenuOpen = false;
    public GameObject pauseMenu;
    bool canPause = false;

    public List<Interactable> interactables = new List<Interactable>();
    public Interactable interactable;

    public bool computerOpen = false;

    public bool[] Canvases = new bool[6];

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(Manager.Instance.AllowInteract)
        {
            interact = context.ReadValueAsButton();
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton() && !canOpenInventory)
        {
            InventoryOpen = !InventoryOpen;
        }
        canOpenInventory = context.ReadValueAsButton();
    }

    public void OnJournal(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton() && !canOpenJournal)
        {
            JournalOpen = !JournalOpen;
        }
        canOpenJournal = context.ReadValueAsButton();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton() && !canPause)
        {
            PauseMenuOpen = !PauseMenuOpen;
        }
        canPause = context.ReadValueAsButton();
    }

    public void Read(string message)
    {
        if (!canRead)
        {
            reading = !reading;
        }
        this.message = message;
        canRead = true;
    }

    public void Speak(string messsage, string speaker)
    {
        speaking = !speaking;
        this.speechMessage = messsage;
        this.speaker = speaker;
    }

    public void Shop()
    {
        if (!canOpenShop)
        {
            shopOpen = !shopOpen;
        }
        canOpenShop = true;
    }

    public void ResetCanRead()
    {
        canRead = false;
    }

    public void ResetCanOpenShop()
    {
        canOpenShop = false;
    }

    public void OnMouseScroll(InputAction.CallbackContext context)
    {
        mouse += context.ReadValue<Vector2>().normalized.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Interactable>() != null)
        {
            interactables.Add(collision.gameObject.GetComponent<Interactable>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Interactable>() != null)
        {
            interactables.Remove(collision.gameObject.GetComponent<Interactable>());
            collision.gameObject.GetComponent<Interactable>().ChangeSelectableView(false);
        }
    }

    public void CheckInventory()
    {
        if (inventory == null || inventory.name != "PlayerMenu")
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
        if (journal == null || journal.name != "PlayerJournal")
        {
            SetJournal();
        }
        EnableDisableJournal();
    }

    public void CheckReading()
    {
        if (readingPage == null || readingPage.name != "Sign")
        {
            SetReadingPage();
        }
        EnableDisableRead();
    }

    public void CheckDialogue()
    {
        if(dialoguePage == null || dialoguePage.name != "SpeechBox")
        {
            SetDialoguePage();
        }
        EnableDisableDialogue();
    }

    public void CheckShop()
    {
        if (shop == null || shop.name != "PlayerShop")
        {
            SetShop();
        }
        EnableDisableShop();
    }

    public void CheckPauseMenu()
    {
        if (pauseMenu == null || pauseMenu.name != "PauseMenu")
        {
            SetPauseMenu();
        }
        EnableDisablePauseMenu();
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
        if (inventory == null || inventory.name != "PlayerMenu") { return; }
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
        if (journal == null || journal.name != "PlayerJournal") { return; }
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

    public void SetPauseMenu()
    {
        if (!pauseMenu && Manager.Instance.settings.isPlayable)
        {
            pauseMenu = GameObject.Find("Canvas");
            if (pauseMenu)
            {
                pauseMenu = pauseMenu.transform.Find("PauseMenu").gameObject;
            }
        }
    }

    public void EnableDisablePauseMenu()
    {
        if (pauseMenu == null || pauseMenu.name != "PauseMenu") { return; }
        foreach (var image in pauseMenu.GetComponentsInChildren<Image>())
        {
            image.enabled = PauseMenuOpen;
        }
        foreach (var image in pauseMenu.GetComponentsInChildren<RawImage>())
        {
            image.enabled = PauseMenuOpen;
        }
        foreach (var text in pauseMenu.GetComponentsInChildren<TMP_Text>())
        {
            text.enabled = PauseMenuOpen;
        }
    }

    public void EnableDisableRead()
    {
        if (readingPage == null || readingPage.name != "Sign") { return; }
        if (reading && readingPage)
        {
            readingPage.GetComponentInParent<ReadPage>().SetMessage(message);
        } else if (readingPage)
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

    public void EnableDisableShop()
    {
        if (shop == null || shop.name != "PlayerShop") { return ; }
        foreach (var image in shop.GetComponentsInChildren<Image>())
        {
            image.enabled = shopOpen;
        }
        foreach (var image in shop.GetComponentsInChildren<RawImage>())
        {
            image.enabled = shopOpen;
        }
        foreach (var text in shop.GetComponentsInChildren<TMP_Text>())
        {
            text.enabled = shopOpen;
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

    public void SetDialoguePage()
    {
        if(!dialoguePage && Manager.Instance.settings.isPlayable)
        {
            dialoguePage = GameObject.Find("Canvas");
            if(dialoguePage)
            {
                dialoguePage = dialoguePage.transform.Find("Dialogue").gameObject;
                if(dialoguePage)
                {
                    dialoguePage = dialoguePage.transform.Find("SpeechBox").gameObject;
                }
            }
        }
    }

    public void EnableDisableDialogue()
    {
        if (dialoguePage == null || dialoguePage.name != "SpeechBox") { return; }
        if (speaking && dialoguePage)
        {
            if(dialoguePage.GetComponentInParent<DialoguePage>() != null)
            {
                dialoguePage.GetComponentInParent<DialoguePage>().SetMessage(speechMessage, speaker);
            }
        }
        foreach (var image in dialoguePage.GetComponentsInChildren<Image>())
        {
            image.enabled = speaking;
        }
        foreach (var image in dialoguePage.GetComponentsInChildren<RawImage>())
        {
            image.enabled = speaking;
        }
        foreach (var text in dialoguePage.GetComponentsInChildren<TMP_Text>())
        {
            text.enabled = speaking;
        }
    }

    public void SetShop()
    {
        if(!shop && Manager.Instance.settings.isPlayable)
        {
            shop = GameObject.Find("Canvas");
            if(shop)
            {
                shop = shop.transform.Find("Shop").gameObject;
                if(shop)
                {
                    shop = shop.transform.Find("PlayerShop").gameObject;
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
        SetPauseMenu();
        SetDialoguePage();
        SetShop();
        if (interactables.Count > 0)
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
        if(pauseMenu)
        {
            CheckPauseMenu();
        }
        if(shop)
        {
            CheckShop();
        }
        if(dialoguePage)
        {
            CheckDialogue();
        }
        bool AllowInteract = true;
        bool AllowMove = true;
        bool AllowOther = true;
        if(!Manager.Instance.settings.Minigame)
        {
            if(Canvases.Length != 6)
            {
                Canvases = new bool[6];
            }
            Canvases[0] = InventoryOpen;
            Canvases[1] = JournalOpen;
            Canvases[2] = reading;
            Canvases[3] = PauseMenuOpen;
            Canvases[4] = shopOpen;
            Canvases[5] = speaking;
            AllowInteract = true;
            AllowMove = true;
            AllowOther = false;
        }
        else
        {
            if(Canvases.Length > 1)
            {
                Canvases = new bool[1];
            }
            Canvases[0] = computerOpen;
            AllowInteract = false;
            AllowMove = false;
            AllowOther = false;
        }
        Manager.Instance.CurrentLoadedCanvasSheet(Canvases, AllowMove, AllowInteract, AllowOther);
    }
}

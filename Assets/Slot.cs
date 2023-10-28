using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class Slot : MonoBehaviour
{
    public Item item;

    public TMP_Text nameText;
    public TMP_Text amountText;
    public RawImage itemImage;

    public bool equipSlot = false;
    public ItemType allowedType;

    public bool equiped;

    public UnityEvent onEquip;

    public int maxObjectsInSlot;
    public int currentInSlot;

    public Slot equipedInSlot;

    public void DisplayInSlot()
    {
        itemImage.texture = item.itemTexture;
        nameText.text = item.itemName;
        if(currentInSlot > 1 && item.itemName != "" && !equipSlot)
        {
            amountText.gameObject.SetActive(true);
            amountText.text = currentInSlot.ToString();
        }else
        {
            amountText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(equiped)
        {
            GetComponent<Button>().interactable = false;
            if(equipedInSlot.currentInSlot !=  currentInSlot)
            {
                equipedInSlot.currentInSlot = currentInSlot;
            }
        }else
        {
            GetComponent<Button>().interactable = true;
        }

        if(equipSlot && item)
        {
            if(equipedInSlot == null)
            {
                try
                {
                    equipedInSlot = Inventory.Instance.FindSlot(item);
                }catch
                {
                    equipedInSlot = Inventory.Instance.GetClosestSlot(item);
                }
            }
            if(Inventory.Instance.FindSlot(item))
            {
                if (Inventory.Instance.FindSlot(item).equipedInSlot == null)
                {
                    Inventory.Instance.FindSlot(item).equipedInSlot = this;
                }
            }else
            {
                UnEquip();
            }
        }
    }

    public void OnClick()
    {
        if(!equipSlot)
        {
            Inventory.Instance.DisplaySlotOptions(this);
        }else if(Inventory.Instance.SelectedSlot != null)
        {
            if(Inventory.Instance.SelectedSlot.item.itemType != allowedType) { return; }
            if(this.item != null)
            {
                Inventory.Instance.FindSlot(item).equiped = false;
            }
            this.item = Inventory.Instance.SelectedSlot.item;
            Inventory.Instance.SelectedSlot.equipedInSlot = this;
            equipedInSlot = Inventory.Instance.SelectedSlot;
            Inventory.Instance.SelectedSlot.equiped = true;
            Inventory.Instance.SelectedSlot = null;
            DisplayInSlot();
            onEquip.Invoke();
        }else if(equipSlot && !Inventory.Instance.SelectedSlot && item)
        {
            UnEquip();
        }
    }

    public void EquipItem(Slot slot)
    {
        if(equipSlot)
        {
            if(!slot.item) { return; }
            if (slot.item.itemType != allowedType) { return; }
            if (this.item != null)
            {
                Inventory.Instance.FindSlot(item).equiped = false;
            }
            this.item = slot.item;
            slot.equipedInSlot = this;
            equipedInSlot = slot;
            slot.equiped = true;
            slot = null;
            DisplayInSlot();
            onEquip.Invoke();
        }
    }

    public void Drop()
    {
        currentInSlot--;
        if(currentInSlot <= 0 && equiped)
        {
            equipedInSlot.UnEquip();
        }
        Inventory.Instance.RemoveItem(item);
        Inventory.Instance.DisplayItems();
    }

    public void UnEquip()
    {
        if(Inventory.Instance.FindSlot(item))
        {
            Inventory.Instance.FindSlot(item).equiped = false;
        }
        if(Inventory.Instance.AmmoSlot == this)
        {
            Inventory.Instance.player.GetComponent<PlayerAttack>().CmdSetAmmo(0);
        }
        if(Inventory.Instance.ConsumableSlot == this)
        {
            Inventory.Instance.player.GetComponent<PlayerAttack>().CmdSetConsumables(0);
        }
        this.item = Inventory.Instance.baseItem;
        DisplayInSlot();
    }
}

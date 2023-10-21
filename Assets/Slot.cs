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

    public void DisplayInSlot()
    {
        itemImage.texture = item.itemTexture;
        nameText.text = item.itemName;
        if(currentInSlot > 1)
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
        }else
        {
            GetComponent<Button>().interactable = true;
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
            Inventory.Instance.SelectedSlot.equiped = true;
            Inventory.Instance.SelectedSlot = null;
            DisplayInSlot();
            onEquip.Invoke();
        }else if(equipSlot && !Inventory.Instance.SelectedSlot && item)
        {
            Inventory.Instance.FindSlot(item).equiped = false;
            this.item = Inventory.Instance.baseItem;
            DisplayInSlot();
        }
    }

    public void Drop()
    {
        currentInSlot--;
        Inventory.Instance.RemoveItem(item);
        Inventory.Instance.DisplayItems();
    }
}

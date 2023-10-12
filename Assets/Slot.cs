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
    public RawImage itemImage;

    public bool equipSlot = false;
    public ItemType allowedType;

    public bool equiped;

    public UnityEvent onEquip;

    public void DisplayInSlot()
    {
        itemImage.texture = item.itemTexture;
        nameText.text = item.itemName;
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
            Inventory.Instance.SelectedSlot = this;
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
}

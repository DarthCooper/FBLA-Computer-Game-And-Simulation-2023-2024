using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Item item;

    public TMP_Text nameText;
    public TMP_Text amountText;
    public RawImage itemImage;

    public float currentInSlot;

    public void DisplayInSlot()
    {
        itemImage.texture = item.itemTexture;
        nameText.text = item.itemName + " x" + currentInSlot;
        amountText.text = item.price.ToString();
    }

    private void Update()
    {
    }

    public void OnClick()
    {
        print("Clicked");
        if((Inventory.Instance.amountOfItem(Shop.instance.currency) - Shop.instance.totalCost) >= item.price && currentInSlot > 0)
        {
            Shop.instance.BuyItem(item);
        }
    }
}

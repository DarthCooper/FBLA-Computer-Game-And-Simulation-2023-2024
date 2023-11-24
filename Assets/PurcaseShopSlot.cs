using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurcaseShopSlot : MonoBehaviour
{
    public Item item;

    public TMP_Text nameText;
    public TMP_Text priceText;

    public int currentInSlot;

    public void DisplayInSlot()
    {
        nameText.text = item.itemName + " x" + currentInSlot;
        priceText.text = (item.price * currentInSlot).ToString();
    }

    private void Update()
    {
        DisplayInSlot();
    }

    public void OnClick()
    {
        
    }
}

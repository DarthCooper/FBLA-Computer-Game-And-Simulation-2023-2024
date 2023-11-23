using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public Item currency;

    public Transform context;
    public ShopSlot slotPrefab;

    public Dictionary<Item, int> items = new Dictionary<Item, int>();
    public List<ShopSlot> slots = new List<ShopSlot>();

    ShopInteract retailer;

    public void AddItem(Item item, int amount, ShopInteract retailer)
    {
        this.retailer = retailer;
        if(!items.ContainsKey(item)) 
        {
            items.Add(item, amount);
        }
        DisplayItems();
    }

    public void BuyItem(Item item)
    {
        if (items.ContainsKey(item))
        {
            items[item]--;
            if (items[item] <= 0)
            {
                items.Remove(item);
            }
        }
        retailer.BuyItem(item);
        DisplayItems();
    }

    public void DisplayItems()
    {
        foreach (ShopSlot slot in slots)
        {
            Destroy(slot.gameObject);
        }
        slots.Clear();
        foreach (var item in items.Keys)
        {
            var slot = Instantiate(slotPrefab, context);
            slot.item = item;
            slot.currentInSlot = items[item];
            slot.DisplayInSlot();
            slots.Add(slot);
        }
    }

    public void Awake()
    {
        instance = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public Item currency;
    [Header("Select Items")]

    public Transform context;
    public ShopSlot slotPrefab;

    public Dictionary<Item, int> items = new Dictionary<Item, int>();
    public List<ShopSlot> slots = new List<ShopSlot>();

    ShopInteract retailer;

    [Header("Purchase Items")]
    public Transform purchaseContext;
    public PurcaseShopSlot purchaseSlot;

    public Dictionary<Item, int> purchaseItems = new Dictionary<Item, int>();
    public List<PurcaseShopSlot> purchaseSlots = new List<PurcaseShopSlot>();

    public int totalCost;

    public void AddItem(Item item, int amount, ShopInteract retailer)
    {
        this.retailer = retailer;
        if(!items.ContainsKey(item)) 
        {
            items.Add(item, amount);
        }
        DisplayItems();
    }

    public void AddpurchaseItem(Item item, int amount)
    {
        if(!purchaseItems.ContainsKey(item))
        {
            purchaseItems.Add(item, amount);
        }
        DisplayPurchaseItems();
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
        AddpurchaseItem(item, 1);
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
            int amount = items[item];
            while(amount > item.maxStack)
            {
                var slot = Instantiate(slotPrefab, context);
                slot.item = item;
                slot.currentInSlot = item.maxStack;
                slot.DisplayInSlot();
                slots.Add(slot);
                amount -= item.maxStack;
            }
            if(amount > 0)
            {
                var slot = Instantiate(slotPrefab, context);
                slot.item = item;
                slot.currentInSlot = amount;
                slot.DisplayInSlot();
                slots.Add(slot);
            }
        }
    }

    public void DisplayPurchaseItems()
    {
        foreach (PurcaseShopSlot slot in purchaseSlots)
        {
            Destroy(slot.gameObject);
        }
        purchaseSlots.Clear();
        foreach(var item in purchaseItems.Keys)
        {
            bool itemExists = false;
            foreach(PurcaseShopSlot purchaseSlot in purchaseSlots)
            {
                if(purchaseSlot.item == item)
                {
                    purchaseSlot.currentInSlot++;
                    itemExists = true;
                }
            }
            if(itemExists) { continue; }
            var slot = Instantiate(purchaseSlot, purchaseContext);
            slot.item = item;
            slot.currentInSlot = purchaseItems[item];
            purchaseSlots.Add(slot);
        }
        foreach(PurcaseShopSlot purchaseSlot in purchaseSlots)
        {
            purchaseSlot.DisplayInSlot();
            totalCost += (purchaseSlot.item.price * purchaseSlot.currentInSlot);
        }
    }

    public void Awake()
    {
        instance = this;
    }

    public void PurchaseItems()
    {
        foreach(var slots in purchaseSlots)
        {
            Inventory.Instance.AddItem(slots.item);
            for (int i = 0; i < totalCost; i++)
            {
                Inventory.Instance.UseItem(currency);
            }
        }
        purchaseItems.Clear();
        DisplayPurchaseItems();
    }
}

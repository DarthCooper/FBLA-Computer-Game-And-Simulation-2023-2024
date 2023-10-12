using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<GameObject> itemSlots = new List<GameObject>();
    public List<Item> items = new List<Item>();

    public GameObject slotPrefab;
    public Transform inventoryContext;

    public Item[] ItemList;

    public Slot SelectedSlot;

    public Slot PrimarySlot;
    public Slot SecondarySlot;

    public Slot Equipable1Slot;
    public Slot Equipable2Slot;
    public Slot Equipable3Slot;
    public Slot Equipable4Slot;

    public Item baseItem;

    private void Awake()
    {
        if(Instance != this)
        {
            Instance = this; 
        }
    }

    public void AddItem(Item item)
    {
        foreach(var Item in ItemList) 
        {
            if(item.itemName == Item.itemName)
            {
                items.Add(Item);
            }
        }
        DisplayItems();
    }

    public Slot FindSlot(Item item)
    {
        return itemSlots[item.inventoryIndex].GetComponent<Slot>();
    }

    public void DisplayItems()
    {
        foreach(GameObject slot in itemSlots)
        {
            Destroy(slot);
        }
        itemSlots.Clear();

        foreach(Item item in items)
        {
            Slot spawnedItem = Instantiate(slotPrefab, inventoryContext).GetComponent<Slot>();
            spawnedItem.item = item;
            spawnedItem.item.inventoryIndex = itemSlots.Count;
            spawnedItem.DisplayInSlot();
            itemSlots.Add(spawnedItem.gameObject);
        }
    }
}

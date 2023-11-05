using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class Inventory : MonoBehaviour, IDataPersistence
{
    public static Inventory Instance;

    [HideInInspector] public GameObject player;

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

    public Slot AmmoSlot;
    public Slot ConsumableSlot;

    public Item baseItem;

    public GameObject slotOptions;
    public Slot questionedSlot;

    public Slider dropSlider;
    public TMP_Text dropText;
    bool dropping;

    private void Awake()
    {
        if (Instance != this)
        {
            Instance = this; 
        }
        player = GameObject.Find("LocalGamePlayer");
    }

    public void AddItem(Item item)
    {
        foreach(var Item in ItemList) 
        {
            if(item.itemName == Item.itemName)
            {
                for(int i = 0; i < item.currentStack; i++)
                {
                    items.Add(Item);
                }
            }
        }
        DisplayItems();
    }

    public Slot FindSlot(Item item)
    {
        try
        {
            return itemSlots[item.inventoryIndex].GetComponent<Slot>();
        }catch
        {
            return null;
        }
    }

    public Item GetItem(string ItemName)
    {
        foreach (var Item in ItemList)
        {
            if(Item.itemName == ItemName)
            {
                return Item;
            }
        }
        return null;
    }

    private void Update()
    {
        if(player != null)
        {
            if(player.GetComponent<PlayerInventory>())
            {
                if(player.GetComponent<PlayerInventory>().PrimaryWeapon != PrimarySlot.item)
                {
                    player.GetComponent<PlayerInventory>().SetPrimaryItem(PrimarySlot.item);
                }
                if(player.GetComponent<PlayerInventory>().SecondaryWeapon != SecondarySlot.item)
                {
                    player.GetComponent<PlayerInventory>().SetSecondaryItem(SecondarySlot.item);
                }
                if(player.GetComponent<PlayerInventory>().Ammo != AmmoSlot.item)
                {
                    player.GetComponent<PlayerInventory>().SetAmmo(AmmoSlot.item);
                }
                if(player.GetComponent<PlayerInventory>().Consumable != ConsumableSlot.item)
                {
                    player.GetComponent<PlayerInventory>().SetConsumable(ConsumableSlot.item);
                }
            }
        }else
        {
            player = GameObject.Find("LocalGamePlayer");
        }

        if(dropping)
        {
            dropText.text = dropSlider.value.ToString();
        }
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
            bool stacked = false;
            if (item.stackable)
            {
                foreach (var slotObject in itemSlots)
                {
                    Slot slot = slotObject.GetComponent<Slot>();
                    if (slot.item.itemName == item.itemName)
                    {
                        if (slot.currentInSlot < slot.maxObjectsInSlot)
                        {
                            slot.currentInSlot++;
                            slot.DisplayInSlot();
                            stacked = true;
                        }
                    }
                } 
            }
            if (!stacked)
            {
                Slot spawnedItem = Instantiate(slotPrefab, inventoryContext).GetComponent<Slot>();
                spawnedItem.item = item;
                spawnedItem.item.inventoryIndex = itemSlots.Count;
                spawnedItem.maxObjectsInSlot = spawnedItem.item.stackable ? spawnedItem.item.maxStack : 1;
                spawnedItem.currentInSlot++;
                spawnedItem.DisplayInSlot();
                itemSlots.Add(spawnedItem.gameObject);
            }
            if(player)
            {
                player.GetComponent<PlayerInteract>().CheckInventory();
            }
        }
    }

    public void DisplaySlotOptions(Slot slot)
    {
        foreach (var itemSlot in itemSlots)
        {
            if(slot == itemSlot.GetComponent<Slot>())
            {
                slotOptions.transform.position = itemSlot.transform.position;
                slotOptions.SetActive(true);
                questionedSlot = slot;
            }
        }
    }

    public void EquipSlot()
    {
        SelectedSlot = questionedSlot;
        questionedSlot = null;
        slotOptions.SetActive(false);
    }

    public void ClostSlotOptions()
    {
        slotOptions.SetActive(false);
        questionedSlot = null;
    }

    public void OnSelectDrop()
    {
        dropSlider.maxValue = questionedSlot.currentInSlot;
        dropping = true;
    }

    public void OnDrop()
    {
        SpawnedDroppedItem((int)dropSlider.value);
        for (int i = 0; i < dropSlider.value; i++)
        {
            questionedSlot.Drop(false);
        }
        dropping = false;
    }

    public void OnDropAll()
    {
        SpawnedDroppedItem(questionedSlot.currentInSlot);
        while(questionedSlot.currentInSlot > 0)
        {
            questionedSlot.Drop(false);
        }
        dropping = false;
    }

    public void UseItem(Item item)
    {
        Slot usedSlot = GetClosestSlot(item);
        if(!usedSlot) { return; }
        usedSlot.GetComponent<Slot>().Drop(true);
    }

    public Slot GetClosestSlot(Item item)
    {
        Slot usedSlot = null;
        foreach (var slot in itemSlots)
        {
            if (slot.GetComponent<Slot>().item)
            {
                if(item)
                {
                    if (slot.GetComponent<Slot>().item.itemName == item.itemName)
                    {
                        usedSlot = slot.GetComponent<Slot>();
                    }
                }
            }
        }
        return usedSlot;
    }

    public void SpawnedDroppedItem(int Amount)
    {
        player.GetComponent<PlayerInventory>().spawnDroppedItem(questionedSlot.item.itemName, Amount);
    }

    public void RemoveItem(Item item)
    {
        foreach (var heldItem in items)
        {
            if(item == heldItem)
            {
                items.Remove(heldItem);
                return;
            }
        }
    }

    public void LoadData(GameData data)
    {
        items.Clear();
        foreach (var itemName in data.items)
        {
            foreach (var item in ItemList)
            {
                if(itemName == item.itemName)
                {
                    items.Add(item);
                }
            }
        }
        DisplayItems();
    }

    public void SaveData(ref GameData data)
    {
        data.items.Clear();
        foreach (var item in items)
        {
            data.items.Add(item.itemName);
        }
    }
}

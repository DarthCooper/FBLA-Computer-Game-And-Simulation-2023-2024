using Mirror;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteract : MonoBehaviour, IDataPersistence
{
    public string ShopName;

    public Item[] item;
    public int[] amount;

    public SerializableDictionary<Item, int> items = new SerializableDictionary<Item, int>();

    public Interactable interactable;

    public Transform interactingPlayer;

    public bool stackItems = true;

    private void Awake()
    {
        for (int i = 0; i < item.Length; i++)
        {
            if (!items.ContainsKey(item[i]))
            {
                items.Add(item[i], amount[i]);
            }
        }
    }

    public void onInteract()
    {
        Transform closest = GetClosest();
        if (closest.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            interactingPlayer = closest;
            interactingPlayer.GetComponent<PlayerInteract>().Shop();

            foreach (var item in items.Keys)
            {
                Shop.instance.AddItem(item, items[item], this);
            }
        }
    }

    public void BuyItem(Item item)
    {
        if(items.ContainsKey(item))
        {
            items[item]--;
            if (items[item] <= 0)
            {
                items.Remove(item);
            }
        }
    }

    public void OnEndInteract()
    {
        if (interactingPlayer)
        {
            interactingPlayer.GetComponent<PlayerInteract>().ResetCanOpenShop();
        }
    }

    public Transform GetClosest()
    {
        float maxDistance = 10000;
        Transform Target = null;
        foreach (var target in GameObject.FindGameObjectsWithTag("Player"))
        {
            float Distance = Vector2.Distance(transform.position, target.transform.position);
            if (Distance < maxDistance)
            {
                maxDistance = Distance;
                Target = target.transform;
            }
        }
        return Target;
    }

    public void LoadData(GameData data)
    {
        if(data.ShopKeepers.ContainsKey(ShopName))
        {
            if (data.ShopKeepers[ShopName].Count > 0)
            {
                items = data.ShopKeepers[ShopName];
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        if(data.ShopKeepers.ContainsKey(ShopName))
        {
            data.ShopKeepers.Remove(ShopName);
        }
        data.ShopKeepers.Add(ShopName, items);
    }
}

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteract : MonoBehaviour
{
    public Item[] item;
    public int[] prices;

    public Dictionary<Item, int> items = new Dictionary<Item, int>();

    public Interactable interactable;

    public Transform interactingPlayer;

    public void onInteract()
    {
        Transform closest = GetClosest();
        if (closest.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            interactingPlayer = closest;
            interactingPlayer.GetComponent<PlayerInteract>().Shop();

            for (int i = 0; i < item.Length; i++)
            {
                if (!items.ContainsKey(item[i]))
                {
                    items.Add(item[i], prices[i]);
                }
            }

            foreach (var item in items.Keys)
            {
                Shop.instance.AddItem(item, items[item]);
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
}

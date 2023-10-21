using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Item : MonoBehaviour
{
    public string itemName;
    public GameObject itemPrefab;
    public Texture2D itemTexture;

    Interactable interactable;

    public ItemType itemType;

    public int inventoryIndex;

    public float delayBetweenUses;

    public bool stackable;
    public int maxStack;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    public void PickUp()
    {
        if(interactable.Player)
        {
            Inventory.Instance.AddItem(this);
        }
        Destroy(this.gameObject);
    }
}
public enum ItemType
{
    Weapon,
    Equipable,
    Useable,
    Ammo,
    Static
}

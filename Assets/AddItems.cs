using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddItems : MonoBehaviour
{
    public Item[] itemsToAdd;

    bool addedItems = false;

    private void Update()
    {
        if(!addedItems)
        {
            if(Inventory.Instance)
            {
                Invoke(nameof(addItems), 3f);
            }
        }
    }

    public void addItems()
    {
        if(addedItems) { return; }
        foreach(var item in itemsToAdd)
        {
            Inventory.Instance.AddItem(item);
        }
        addedItems = true;
    }
}

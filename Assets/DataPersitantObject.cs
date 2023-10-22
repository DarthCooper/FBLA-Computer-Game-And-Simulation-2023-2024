using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersitantObject : MonoBehaviour, IDataPersistence
{
    public SerializableDictionary<string, bool> itemsCollected;

    public List<Interactable> items = new List<Interactable>();

    public void LoadData(GameData data)
    {
        this.itemsCollected = data.itemsCollected;
        foreach (var id in itemsCollected.Keys)
        {
            foreach (var item in items) 
            {
                if(id == item.id)
                {
                    item.collected = itemsCollected[id];
                    if(item.collected)
                    {
                        item.GetComponent<Item>().DisableObject();
                    }
                }
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        foreach(var item in items)
        {
            if (data.itemsCollected.ContainsKey(item.id))
            {
                data.itemsCollected.Remove(item.id);
            }
            data.itemsCollected.Add(item.id, item.collected);
        }
    }
}

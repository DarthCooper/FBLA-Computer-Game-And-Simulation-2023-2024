using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    public static Journal Instance;

    [HideInInspector]public GameObject player;

    public List<Quest> questSteps = new List<Quest>();
    public List<GameObject> questSlots = new List<GameObject>();

    public GameObject questSlot;

    public Transform context;

    private void Awake()
    {
        if(Instance != this)
        {
            Instance = this;
        }
        player = GameObject.Find("LocalGamePlayer");
    }

    public void AddQuest(Quest quest)
    {
        questSteps.Add(quest);
        DisplayQuests();
    }

    public void RemoveQuest(Quest quest)
    {
        questSteps.Remove(quest);
        DisplayQuests();
    }

    public void DisplayQuests()
    {
        foreach (GameObject quest in questSlots)
        {
            Destroy(quest);
        }
        questSlots.Clear();

        foreach (Quest quest in questSteps)
        {
            QuestSlot slot = Instantiate(questSlot, context).GetComponent<QuestSlot>();
            slot.questName = quest.info.displayName;
            if(quest.CurrentStepExists())
            {
                slot.step = quest.GetCurrentStep();
            }
            slot.SetValues();
            questSlots.Add(slot.gameObject);
        }
        player.GetComponent<PlayerInteract>().EnableDisableJournal();
    }
}

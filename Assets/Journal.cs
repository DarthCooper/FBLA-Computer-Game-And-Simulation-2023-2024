using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Journal : MonoBehaviour, IDataPersistence
{
    public static Journal Instance;

    [HideInInspector]public GameObject player;

    public List<Quest> questSteps = new List<Quest>();
    public List<QuestSlot> questSlots = new List<QuestSlot>();

    public GameObject questSlot;

    public QuestSlot selectedSlot;

    public Transform context;

    public TMP_Text questNameText;
    public TMP_Text questProgressText;

    public Transform arrowImage;

    private string lastSelectedQuestName;

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
        if(!quest.info.previouslyLoaded)
        {
            if(quest.info.autoEquip)
            {
                if(selectedSlot != null)
                {
                    fullyUnselectQuest();
                }
                lastSelectedQuestName = quest.info.displayName;
            }
        }
        questSteps.Add(quest);
        DisplayQuests();
    }

    public void RemoveQuest(Quest quest)
    {
        questSteps.Remove(quest);
        DisplayQuests();
    }

    public void RemoveQuest(string questID)
    {
        List<Quest> tempQuests = new List<Quest>();
        bool questRemoved = false;
        foreach(Quest quest in questSteps)
        {
            if(quest.info.id != questID)
            {
                questRemoved = true;
                tempQuests.Add(quest);
            }
        }
        if(questRemoved)
        {
            questSteps = tempQuests;
            DisplayQuests();
        }
    }

    public void DisplayQuests()
    {
        foreach (QuestSlot quest in questSlots)
        {
            Destroy(quest.gameObject);
        }
        questSlots.Clear();

        foreach (Quest quest in questSteps)
        {
            QuestSlot slot = Instantiate(questSlot, context).GetComponent<QuestSlot>();
            slot.name = slot.name + questSlots.Count.ToString();
            slot.questName = quest.info.displayName;
            if(quest.CurrentStepExists())
            {
                slot.step = quest.GetCurrentStep();
            }
            slot.maxProgress = quest.info.questStepPrefabs[quest.info.questStepPrefabs.Length - 1].GetComponent<QuestStep>().maxProgress;
            slot.SetValues();
            slot.GetComponent<Button>().onClick.AddListener(slot.SelectQuest);
            if(slot.questName == lastSelectedQuestName)
            {
                SelectQuest(slot);
            }
            questSlots.Add(slot);
        }
        player.GetComponent<PlayerInteract>().EnableDisableJournal();
    }

    private void Update()
    {
        DestroyDuplicated();
        if (selectedSlot == null)
        {
            questNameText.text = string.Empty;
            questProgressText.text = string.Empty;
            arrowImage.gameObject.SetActive(false);
        }
        else
        {
            questNameText.text = selectedSlot.questName;
            questProgressText.text = selectedSlot.progressText.text;
            if (selectedSlot.usesArrow)
            {
                arrowImage.gameObject.SetActive(true);
                float angle = Mathf.Atan2(selectedSlot.step.difference.y, selectedSlot.step.difference.x) * Mathf.Rad2Deg;
                arrowImage.rotation = Quaternion.Euler(new Vector3(0,0, angle));
            }
            else
            {
                arrowImage.gameObject.SetActive(false);
            }
        }
    }

    public void DestroyDuplicated()
    {
        bool duplicateFound = false;
        List<Quest> duplicates = new List<Quest>();
        for (int i = 0; i < questSteps.Count; i++)
        {
            for (int j = 0; j < questSteps.Count; j++)
            {
                if(i != j && questSteps[i].info.id == questSteps[j].info.id)
                {
                    if (duplicates.Contains(questSteps[j])) { continue; }
                    duplicates.Add(questSteps[j]);
                    duplicateFound = true;
                }
            }
        }
        if(duplicateFound)
        {
            foreach(var duplicate in duplicates)
            {
                RemoveQuest(duplicate);
            }
        }
    }

    public void SelectQuest(QuestSlot slot)
    {
        selectedSlot = slot;
        questNameText.text = selectedSlot.questName;
        questProgressText.text = selectedSlot.progressText.text;
        lastSelectedQuestName = selectedSlot.questName;
    }

    public void unselectQuest()
    {
        selectedSlot = null;
        questNameText.text = string.Empty;
        questProgressText.text = string.Empty;
        lastSelectedQuestName = string.Empty;
    }

    public void fullyUnselectQuest()
    {
        selectedSlot = null;
        questNameText.text = string.Empty;
        questProgressText.text = string.Empty;
        lastSelectedQuestName = string.Empty;
        lastSelectedQuestName = string.Empty;
    }

    public void LoadData(GameData data)
    {
        this.lastSelectedQuestName = data.selectedQuest;
    }

    public void SaveData(ref GameData data)
    {
        data.selectedQuest = this.lastSelectedQuestName;
    }
}

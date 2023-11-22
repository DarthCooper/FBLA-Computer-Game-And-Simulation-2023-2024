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
        if(selectedSlot != null)
        {
            unselectQuest();
        }
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
        foreach (QuestSlot quest in questSlots)
        {
            Destroy(quest.gameObject);
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
        if(selectedSlot == null)
        {
            questNameText.text = string.Empty;
            questProgressText.text = string.Empty;
        }else
        {
            questNameText.text = selectedSlot.questName;
            questProgressText.text = selectedSlot.progressText.text;
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
